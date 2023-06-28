using Godot;

namespace GolfGame
{
    public class MenuLayoutManager : Control
    {
        LoadingManager _loadingManager;
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

        private void HostNewGame_Response(string code = null)
        {
            if (code == null)
            {
                // @todo Show server error.
                GD.PrintErr("No code received from server.");
                LoadingComplete();
                return;
            }

            _hostingCode.Text = code;
            _hosting.Visible = true;
            LoadingComplete();
            return;
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

        public void _OnBotsButtonDown()
        {
            ShowFriendsPanel();
        }

        public void _OnFriendsButtonDown()
        {
            ShowFriendsPanel();
        }

        public void _OnFriendsBackButtonDown()
        {
            ShowHomePanel();
        }

        public void _OnJoinGameButtonDown()
        {
            ShowCodePanel();
        }

        public void _OnJoinGameBackButtonDown()
        {
            ShowFriendsPanel();
        }

        public void _OnHostGameButtonDown()
        {
            ShowHostingPanel();
        }

        public void _OnHostGameBackButtonDown()
        {
            ShowFriendsPanel();
        }

        private void EvaluateWebsocketResponses(WebSocket.WebSocketResponse response)
        {
            switch (response.RequestType)
            {
                case WebSocketRequestType.HostNewGame:
                    HostNewGame_Response(response.data);
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
