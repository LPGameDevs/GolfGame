using GameCore;
using Godot;

namespace GolfGame.Helpers
{
    public class WebSocketGameEventHandler
    {
        GameManager _gameManager;
        WebSocket _webSocket;

        public WebSocketGameEventHandler(GameManager gameManager, WebSocket webSocket)
        {
            _gameManager = gameManager;
            _webSocket = webSocket;
        }

        public void HandleResponse(WebSocket.WebSocketResponse response)
        {
            switch (response.responseType)
            {
                case WebSocketResponseType.CardDrawn:
                case WebSocketResponseType.CardPlaced:
                case WebSocketResponseType.PlayerKnocked:
                case WebSocketResponseType.TurnCompleted:
                    // Data is the user performing the action.
                    UserActionConfirmed_Response(response.responseType, response.user, response.game);
                    break;
                case WebSocketResponseType.InteractionError:
                    // Data is the kind of action that failed.
                    UserActionError_Response(response.data);
                    break;
            }
        }

        private void UserActionError_Response(string interaction)
        {
            // @todo Tell user that requested action failed.
            throw new System.NotImplementedException();
        }

        private void UserActionConfirmed_Response(WebSocketResponseType webSocketResponseType, string uid, GameState game)
        {
            _gameManager.ConfirmUserAction(uid, webSocketResponseType, game);
        }

    }
}
