using GameCore;
using Godot;

namespace GolfGame.Helpers
{
    public class WebSocketMenuEventHandler
    {
        GameManager _gameManager;
        MenuLayoutManager _parent;
        WebSocket _webSocket;

        public WebSocketMenuEventHandler(MenuLayoutManager parent, GameManager gameManager, WebSocket webSocket)
        {
            // @todo Remove the parent dependency.
            _parent = parent;
            _gameManager = gameManager;
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
                case WebSocketResponseType.GameStarted:
                    // Data is the kind of action that failed.
                    StartGame_Response(response.game);
                    break;
            }
        }

        private void StartGame_Response(GameState gameState)
        {
            _parent.StartGameSuccess(gameState);
        }

        private void Handshake_Response(string responseUser)
        {
            _gameManager.CurrentUser = responseUser;
            _webSocket.HandshakeSuccess();
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
