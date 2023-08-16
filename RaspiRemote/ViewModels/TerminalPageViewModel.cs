using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RaspiRemote.Parsers;
using Renci.SshNet;
using System.Net;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace RaspiRemote.ViewModels
{
    internal partial class TerminalPageViewModel : BaseViewModel
    {
        private readonly SshClient _sshClient;
        private ShellStream _shellStream;
        private readonly WebSocketServer _webSockServer;

        public event Action<string> ConsoleDataReceived;
        public bool IsConsoleInitialized { get; set; } = false;
        public (double, double) ConsoleDimensions { private get; set; } = (0, 0);

        [ObservableProperty]
        private string _commandText;

        private bool _sendCtrlKey;
        public bool SendCtrlKey
        {
            get => _sendCtrlKey;
            set
            {
                if (SendFnKey) SendFnKey = false;
                _sendCtrlKey = value;
                OnPropertyChanged(nameof(SendCtrlKey));
            }
        }

        private bool _sendFnKey;
        public bool SendFnKey
        {
            get => _sendFnKey;
            set
            {
                if (SendCtrlKey) SendCtrlKey = false;
                _sendFnKey = value;
                OnPropertyChanged(nameof(SendFnKey));
            }
        }

        public TerminalPageViewModel(SshClientContainer sshClientContainer)
        {
            _sshClient = sshClientContainer.SshClient;
            _webSockServer = new("ws://localhost:8880");

            _ = ConfigureShellStream();
        }

        private async Task ConfigureShellStream() => await InvokeAsyncWithLoader(() =>
        {
            //while (ConsoleDataReceived is null || ConsoleDimensions == (0, 0))
            while (IsConsoleInitialized is false)
            {
                Thread.Sleep(100); // waiting for console view to initialize
            }

            try
            {
                var colsAndRows = GetColsAndRows();
                _shellStream = _sshClient.CreateShellStream("xterm", colsAndRows.Item1, colsAndRows.Item2, 0, 0, 0);
                SetupShellStreamEvents();
            }
            catch (Exception ex)
            {
                _ = DisplayError(ex.Message);
            }
        });

        private (uint, uint) GetColsAndRows()
        {
            var cellWidth = 7;
            var cellHeight = 14;

#if WINDOWS
            // 15 is the width of scrollbar, 2 is a margin of error
            var consoleCols = (uint)((ConsoleDimensions.Item1 - 15) / cellWidth) - 2;
            var consoleRows = (uint)(ConsoleDimensions.Item2 / cellHeight);
#elif ANDROID
            // 5 is a margin of error
            var consoleCols = (uint)(ConsoleDimensions.Item1 / cellWidth) - 5;
            // 0.5 is a number representing how much of the screen the console takes while soft keyboard is open (more or less)
            var consoleRows = (uint)(ConsoleDimensions.Item2 * 0.4 / cellHeight);
#endif

            return (consoleCols, consoleRows);
        }

        private class MyWebSocket : WebSocketBehavior
        {
            public ShellStream ShellStream { get; set; }

            protected override void OnMessage(MessageEventArgs e)
            {
                ShellStream?.Write(e.Data);
            }

            public new void Send(byte[] data) => base.Send(data);
        }

        private void SetupShellStreamEvents()
        {
            _webSockServer.AddWebSocketService<MyWebSocket>("/shell", (webSock) =>
            {
                webSock.ShellStream = _shellStream;

                //_shellStream.DataReceived += (s, e) =>
                //{
                //    if (_shellStream.DataAvailable)
                //    {
                //        Span<byte> buffer = new();
                //        _shellStream.Read(buffer);
                //        webSock.Send(buffer.ToArray());
                //    }
                //};
            });

            _webSockServer.Start();

            _shellStream.DataReceived += (s, e) =>
            {
                if (_shellStream.DataAvailable)
                {
                    var data = _shellStream.Read();
                    ConsoleDataReceived?.Invoke(data);
                }
            };

            _shellStream.ErrorOccurred += (s, e) =>
            {
                _ = DisplayAlert("Error", e.Exception.Message, "OK");
            };
        }

        [RelayCommand]
        private void HandleSendBtn(string mode)
        {
            try
            {
                if (SendCtrlKey)
                {
                    var ctrlKey = CtrlCharacterParser.GetCtrlCharacter(CommandText);
                    Send(ctrlKey.ToString());
                    SendCtrlKey = false;
                }
                else if (SendFnKey)
                {
                    var fnKeyCode = FnKeyCodeParser.GetFnKeyCode(CommandText);
                    Send(fnKeyCode);
                    SendFnKey = false;
                }
                else
                {
                    if (mode == "Send") Send(CommandText);
                    else if (mode == "Execute") SendLine(CommandText);
                }
                CommandText = string.Empty;
            }
            catch (Exception ex)
            {
                _ = DisplayError(ex.Message);
            }
        }

        private void SendLine(string cmdText)
        {
            try
            {
                _shellStream.WriteLine(cmdText);
            }
            catch (ObjectDisposedException)
            {
                _ = HandleShellInterrupted();
            }
            catch (Exception ex)
            {
                _ = DisplayError(ex.Message);
            }
        }

        private void Send(string cmdText)
        {
            try
            {
                _shellStream.Write(cmdText);
            }
            catch (ObjectDisposedException)
            {
                _ = HandleShellInterrupted();
            }
            catch (Exception ex)
            {
                _ = DisplayError(ex.Message);
            }
        }

        private async Task HandleShellInterrupted()
        {
            var result = await DisplayAlert("Shell interrupted", "Do you want to reconnect to the shell?", "Yes", "No");
            if (result)
            {
                _ = ConfigureShellStream();
            }
        }

        [RelayCommand]
        private void Up() => Send("\x1B[A");

        [RelayCommand]
        private void Down() => Send("\x1B[B");

        [RelayCommand]
        private void Left() => Send("\x1B[D");

        [RelayCommand]
        private void Right() => Send("\x1B[C");

        [RelayCommand]
        private void Backspace() => Send("\x08");

        [RelayCommand]
        private void Tab() => Send("\x09");

        [RelayCommand]
        private void Esc() => Send("\x1B");

        [RelayCommand]
        private void ChangeSendFnKey() => SendFnKey = !SendFnKey;

        [RelayCommand]
        private void ChangeSendCtrlKey() => SendCtrlKey = !SendCtrlKey;
    }
}
