using Godot;

namespace GolfGame.Helpers
{
    public class WebSocketRequestHandler
    {
        GameManager _gameManager;
        LoadingManager _loadingManager;
        MenuLayoutManager _parent;
        WebSocket _webSocket;

        public WebSocketRequestHandler(MenuLayoutManager parent, GameManager gameManager, LoadingManager loadingManager, WebSocket webSocket)
        {
            // @todo Remove the parent dependency.
            _parent = parent;
            _gameManager = gameManager;
            _loadingManager = loadingManager;
            _webSocket = webSocket;
        }

        public void HandleRequest(WebSocketRequestType requestType, string data = "")
        {
           _webSocket.MakeRequest(requestType, data);
        }

        public void HostGame()
        {
            _webSocket.MakeRequest(WebSocketRequestType.HostNewGame);
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
