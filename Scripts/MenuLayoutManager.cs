using Godot;

namespace GolfGame
{
    public class MenuLayoutManager : Control
    {
        LoadingManager _loadingManager;
        GameManager _gameManager;
        WebSocket _webSocket;

        private Control _startButtons;
        private Control _friendsButtons;
        private Control _enterCode;
        private Control _hosting;
        private LineEdit _hostingCode;


        public override void _Ready()
        {
            _loadingManager = GetNode<LoadingManager>("/root/LoadingManager");
            _webSocket = GetNode<WebSocket>("/root/WebSocket");
            _gameManager = GetNode<GameManager>("/root/GameManager");

            _startButtons = GetNode<Control>("StartButtons");
            _friendsButtons = GetNode<Control>("FriendsButtons");
            _enterCode = GetNode<Control>("EnterCode");
            _hosting = GetNode<Control>("Hosting");
            _hostingCode = _hosting.GetNode<LineEdit>("Code");
            CallDeferred(nameof(ShowHomePanel));
        }

        private void HostNewGame()
        {
            // Webhook send new game request.
            LoadingStart();

            _webSocket.MakeRequest(WebSocketRequestType.HostNewGame);
        }

        private void LeaveHostGame()
        {
            LoadingStart();
            string code = _gameManager.CurrentRoom;
            _webSocket.MakeRequest(WebSocketRequestType.LeaveGame, code);
        }

        private void HostNewGame_Response(string code = null)
        {
            if (code == null)
            {
                // @todo Show server error.
                GD.PrintErr("No code received from server.");
                LoadingComplete();
                return;
            }

            _gameManager.JoinRoom(code);

            _hostingCode.Text = code;
            _hosting.Visible = true;
            LoadingComplete();
        }

        private void LeaveHostGame_Response()
        {
            _gameManager.LeaveRoom();
            LoadingComplete();
        }

        private void LoadingStart()
        {
            _loadingManager.ShowLoading();
        }

        private void LoadingComplete()
        {
            _loadingManager.HideLoading();
        }

        private void HideAll()
        {
            _startButtons.Visible = false;
            _friendsButtons.Visible = false;
            _enterCode.Visible = false;
            _hosting.Visible = false;
        }

        private void ShowHomePanel()
        {
            HideAll();
            _startButtons.Visible = true;

            LoadingDebug();
        }

        private void ShowFriendsPanel()
        {
            HideAll();
            _friendsButtons.Visible = true;

            LoadingDebug();
        }

        private void ShowCodePanel()
        {
            HideAll();
            _enterCode.Visible = true;
            LoadingDebug();
        }

        private void ShowHostingPanel()
        {
            HideAll();
            HostNewGame();
        }


        private void LoadingDebug()
        {
            _loadingManager.ShowLoadingTimed(0.5f);
        }

        public void _OnButtonDown_Start_Bots()
        {
            ShowFriendsPanel();
        }

        public void _OnButtonDown_Start_Friends()
        {
            ShowFriendsPanel();
        }

        public void _OnButtonDown_Friends_Join()
        {
            ShowCodePanel();
        }

        public void _OnButtonDown_Friends_Host()
        {
            ShowHostingPanel();
        }

        public void _OnButtonDown_Friends_Back()
        {
            ShowHomePanel();
        }

        public void _OnButtonDown_Join_Start()
        {
            GD.Print("Join game.");
        }

        public void _OnButtonDown_Join_Back()
        {
            ShowFriendsPanel();
        }

        public void _OnButtonDown_Host_Start()
        {
            GD.Print("Start game.");
        }

        public void _OnButtonDown_Host_Back()
        {
            LeaveHostGame();
            ShowFriendsPanel();
        }

        private void EvaluateWebsocketResponses(WebSocket.WebSocketResponse response)
        {
            switch (response.responseType)
            {
                case WebSocketResponseType.UserJoinedRoom:
                    HostNewGame_Response(response.data);
                    break;
                case WebSocketResponseType.UserLeftRoom:
                    LeaveHostGame_Response();
                    break;
            }
        }

        public override void _EnterTree()
        {
            WebSocket.OnResponseReceived += EvaluateWebsocketResponses;
        }

        public override void _ExitTree()
        {
            WebSocket.OnResponseReceived -= EvaluateWebsocketResponses;

        }
    }
}
