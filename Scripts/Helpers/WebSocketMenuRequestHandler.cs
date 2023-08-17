namespace GolfGame.Helpers
{
    public class WebSocketMenuRequestHandler
    {
        GameManager _gameManager;
        WebSocket _webSocket;

        public WebSocketMenuRequestHandler(GameManager gameManager, WebSocket webSocket)
        {
            _gameManager = gameManager;
            _webSocket = webSocket;
        }

        public void HostGame()
        {
            _webSocket.MakeRequest(WebSocketRequestType.HostNewGame);
        }

        public void StartGame(string code)
        {
            _webSocket.MakeRequest(WebSocketRequestType.StartGame, code);
        }

        public void JoinGame(string code)
        {
            _webSocket.MakeRequest(WebSocketRequestType.JoinGame, code);
        }

        public void LeaveGame(string code)
        {
            _webSocket.MakeRequest(WebSocketRequestType.LeaveGame, code);
        }
    }
}
