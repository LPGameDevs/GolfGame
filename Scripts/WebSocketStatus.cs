using Godot;

namespace GolfGame
{
    public class WebSocketStatus : Control
    {
        private GameManager _gameManager;

        private Label _statusLabel;
        private Label _uidLabel;

        public override void _Ready()
        {
            _gameManager = GetNode<GameManager>("/root/GameManager");

            _statusLabel = GetNode<Label>("_StatusLabel");
            _uidLabel = GetNode<Label>("_UidLabel");
        }

        private void OnConnected()
        {
            _statusLabel.Text = "Connected";
            _uidLabel.Text = _gameManager.CurrentUser;
        }

        private void OnDisconnected()
        {
            _statusLabel.Text = "Disconnected";
            _uidLabel.Text = "";
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
