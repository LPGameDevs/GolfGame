using Godot;

namespace GolfGame.Helpers
{
    public class WebSocketGameRequestHandler
    {
        GameManager _gameManager;
        WebSocket _webSocket;

        public WebSocketGameRequestHandler(GameManager gameManager, WebSocket webSocket)
        {
            _gameManager = gameManager;
            _webSocket = webSocket;
        }

        public void DrawCardDeck(string gid)
        {
            _webSocket.MakeRequest(WebSocketRequestType.DrawCardDeck, gid);
        }

        public void DrawCardDiscard(string gid)
        {
            _webSocket.MakeRequest(WebSocketRequestType.DrawCardDiscard, gid);
        }

        public void PlaceHandCard(string gid, int index)
        {
            _webSocket.MakeRequest(WebSocketRequestType.PlaceHandCard, gid + ":" + index);
        }

        public void DiscardHandCard(string gid)
        {
            _webSocket.MakeRequest(WebSocketRequestType.DiscardHandCard, gid);
        }

    }
}
