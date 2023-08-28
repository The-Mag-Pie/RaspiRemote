using CommunityToolkit.Mvvm.Input;
using RaspiRemote.Parsers;
using RaspiRemote.WebSocket;
using Renci.SshNet;
using WebSocketSharp.Server;

namespace RaspiRemote.ViewModels
{
    internal partial class TerminalPageViewModel : BaseViewModel, IDisposable
    {
        private readonly SshClient _sshClient;
        private ShellStream _shellStream;
        private readonly WebSocketServer _webSockServer;

        public bool IsWebViewLoaded { get; set; } = false;
        public Action InitializeConsoleFunction { private get; set; }
        public Action ReloadConsoleFunction { private get; set; }
        public (double, double) ConsoleDimensions { private get; set; } = (0, 0);

        public TerminalPageViewModel(SshClientContainer sshClientContainer)
        {
            _sshClient = sshClientContainer.SshClient;
            _webSockServer = new("ws://localhost:8880");
            _webSockServer.ReuseAddress = true;

            sshClientContainer.Disconnecting += Dispose;

            _ = Configure();
        }

        public void OnConsoleSizeChanged()
        {
            var colsAndRows = GetColsAndRows();
            _shellStream?.SendWindowChangeRequest(colsAndRows.Item1, colsAndRows.Item2, 0, 0);
        }

        private async Task Configure() => await InvokeAsyncWithLoader(() =>
        {
            while (IsWebViewLoaded is false)
            {
                Thread.Sleep(100); // waiting for webview to load
            }

            try
            {
                var colsAndRows = GetColsAndRows();
                _shellStream = _sshClient.CreateShellStream("xterm", colsAndRows.Item1, colsAndRows.Item2, 0, 0, 0);
                SetupShellStream();

                SetupWebSocket();

                InitializeConsoleFunction.Invoke();
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
            // 15 is the width of scrollbar, 2 is a margin of error on Windows
            var consoleCols = (uint)((ConsoleDimensions.Item1 - 15) / cellWidth) - 2;
#elif ANDROID
            // 5 is a margin of error on Android
            var consoleCols = (uint)(ConsoleDimensions.Item1 / cellWidth) - 5;
#endif
            var consoleRows = (uint)(ConsoleDimensions.Item2 / cellHeight);

            return (consoleCols, consoleRows);
        }

        private void SetupShellStream()
        {
            _shellStream.ErrorOccurred += (s, e) =>
            {
                _ = DisplayAlert("Error", e.Exception.Message, "OK");
            };
        }

        private void SetupWebSocket()
        {
            _webSockServer.AddWebSocketService<ShellWebSocket>("/shell", (webSock) =>
            {
                webSock.ShellStream = _shellStream;
                webSock.MessageReceived += Send;
            });

            _webSockServer.Start();
        }

        private void Send(string data, ShellStream shellStream)
        {
            try
            {
                shellStream.Write(data);
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

        private void Send(string data) => Send(data, _shellStream);

        private async Task HandleShellInterrupted() => await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            var result = await DisplayAlert("Shell disconnected", "Do you want to reconnect to the shell?", "Yes", "No");
            if (result)
            {
                ResetConsole();
            }
        });

        private void ResetConsole()
        {
            Dispose();

            IsWebViewLoaded = false;
            ReloadConsoleFunction.Invoke();

            _ = Configure();
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
        private async Task FnKey()
        {
            var key = await DisplayPromptAsync("Fn key", "Enter Fn key number (0-12)", "Send", keyboard: Keyboard.Numeric);
            if (key is null) return;

            string fnKey;
            try
            {
                fnKey = FnKeyCodeParser.GetFnKeyCode(key);
            }
            catch (Exception ex)
            {
                _ = DisplayError(ex.Message);
                return;
            }

            Send(fnKey);
        }

        [RelayCommand]
        private async Task CtrlKey()
        {
            var key = await DisplayPromptAsync("Ctrl key", "Enter Ctrl key character (Ctrl+...)", "Send");
            if (key is null) return;

            char ctrlKey;
            try
            {
                ctrlKey = CtrlCharacterParser.GetCtrlCharacter(key);
            }
            catch (Exception ex)
            {
                _ = DisplayError(ex.Message);
                return;
            }

            Send(ctrlKey.ToString());
        }

        public void Dispose()
        {
            _webSockServer?.Stop();
            _webSockServer?.RemoveWebSocketService("/shell");

            _shellStream?.Close();
            _shellStream?.Dispose();
            _shellStream = null;
        }
    }
}
