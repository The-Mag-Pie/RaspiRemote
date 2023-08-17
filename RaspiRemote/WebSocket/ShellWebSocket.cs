using Renci.SshNet;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace RaspiRemote.WebSocket
{
    internal class ShellWebSocket : WebSocketBehavior
    {
        private ShellStream _shellStream;
        /// <summary>
        /// ShellStream used by WebSocket
        /// </summary>
        public ShellStream ShellStream
        {
            private get => _shellStream;
            set
            {
                _shellStream = value;
                if (value is not null) ConfigureShellStream();
            }
        }

        /// <summary>
        /// Event fired when websocket incoming message has been received.
        /// </summary>
        public event Action<string, ShellStream> MessageReceived;

        protected override void OnMessage(MessageEventArgs e) =>
            MessageReceived?.Invoke(e.Data, ShellStream);

        private void ConfigureShellStream()
        {
            _shellStream.DataReceived += (s, e) =>
            {
                // do nothing when websocket is closing or closed
                if (State == WebSocketState.Closing || State == WebSocketState.Closed)
                    return;

                while (State == WebSocketState.Connecting)
                {
                    Thread.Sleep(100); //wait for websocket to connect
                }

                if (_shellStream.DataAvailable)
                {
                    Send(_shellStream.Read());
                }
            };
        }
    }
}
