using Godot;

namespace GolfGame
{
    public class WebSocketStatus : Control
    {
        private Label _statusLabel;

        public override void _Ready()
        {
            _statusLabel = GetNode<Label>("_StatusLabel");
        }

        private void OnConnected()
        {
            _statusLabel.Text = "Connected";
        }

        private void OnDisconnected()
        {
            _statusLabel.Text = "Disconnected";
        }

        public override void _EnterTree()
        {
            WebSocket.OnConnected += OnConnected;
            WebSocket.OnDisconnected += OnDisconnected;
        }

        public override void _ExitTree()
        {
            WebSocket.OnConnected -= OnConnected;
            WebSocket.OnDisconnected -= OnDisconnected;
        }
    }
}
