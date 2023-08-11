using Godot;

namespace GolfGame.Helpers
{
    public class WebSocketEventHandler
    {
        GameManager _gameManager;
        LoadingManager _loadingManager;
        MenuLayoutManager _parent;
        WebSocket _webSocket;

        public WebSocketEventHandler(MenuLayoutManager parent, GameManager gameManager, LoadingManager loadingManager, WebSocket webSocket)
        {
            // @todo Remove the parent dependency.
            _parent = parent;
            _gameManager = gameManager;
            _loadingManager = loadingManager;
            _webSocket = webSocket;
        }

        public void HandleResponse(WebSocket.WebSocketResponse response)
        {
            switch (response.responseType)
            {
                case WebSocketResponseType.UserHandshake:
                    Handshake_Response(response.user);
                    break;
                case WebSocketResponseType.UserJoinedRoom:
                    // @todo joining a room could be comms or game related.
                    // This needs additional data. Which user.
                    // Data is the room that was joined.
                    JoinGame_Response(response.room, response.user, response.users);
                    break;
                case WebSocketResponseType.UserLeftRoom:
                    // This needs additional data. Which room and which user.
                    LeaveGame_Response(response.room, response.user, response.users);
                    break;
                case WebSocketResponseType.UserConnected:
                case WebSocketResponseType.UserDisconnected:
                    // @todo update friends list if relevant.
                    break;
                case WebSocketResponseType.CardPicked:
                case WebSocketResponseType.CardPlaced:
                case WebSocketResponseType.PlayerKnocked:
                    // Data is the user performing the action.
                    UserActionConfirmed_Response(response.responseType, response.user);
                    break;
                case WebSocketResponseType.InteractionError:
                    // Data is the kind of action that failed.
                    UserActionError_Response(response.data);
                    break;
            }
        }

        private void Handshake_Response(string responseUser)
        {
            _gameManager.CurrentUser = responseUser;
            _webSocket.HandshakeSuccess();
        }

        private void UserActionError_Response(string interaction)
        {
            // @todo Tell user that requested action failed.
            throw new System.NotImplementedException();
        }

        private void UserActionConfirmed_Response(WebSocketResponseType webSocketResponseType, string uid)
        {
            // @todo Tell user that requested action succeeded.
            throw new System.NotImplementedException();
        }

        private void JoinGame_Response(string code = null, string uid = null, string[] users = null)
        {
            // This is a response for:
            // - Joining a game.
            // - Hosting a game.

            if (code == null)
            {
                // @todo Show server error.
                GD.PrintErr("No code received from server.");
                _parent.JoinGameFailure();
                return;
            }

            if (users == null || users.Length == 0)
            {
                // @todo Show server error.
                GD.PrintErr("No users received from server.");
                _parent.JoinGameFailure();
                return;
            }

            if (uid == _gameManager.CurrentUser)
            {
               ThisUserJoined();
            }
            else
            {
                OtherUserJoined();
            }


            void ThisUserJoined()
            {

                _gameManager.JoinRoom(code);
                _parent.JoinGameSuccess(code, users);
            }
            void OtherUserJoined()
            {
                _parent.JoinGameSuccess(code, users);
            }
        }


        private void LeaveGame_Response(string room = null, string userLeft = null, string[] users = null)
        {
            // This is a response for:
            // - Leaving a game pre start.
            // - Quiting a game post start.

            if (userLeft == _gameManager.CurrentUser)
            {
                _gameManager.LeaveRoom();
                _parent.LeaveGameSuccess();
            }
            else
            {
                _parent.JoinGameSuccess(room, users);
            }

        }

    }
}
