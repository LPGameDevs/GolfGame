using GameCore;
using Godot;
using GolfGame.Helpers;

namespace GolfGame
{
    public class MenuLayoutManager : Control
    {
        LoadingManager _loadingManager;
        GameManager _gameManager;
        WebSocket _webSocket;

        WebSocketEventHandler _eventHandler;
        WebSocketRequestHandler _requestHandler;

        private Control _startButtons;
        private Control _friendsButtons;
        private Control _enterCode;
        private LineEdit _enterCodeInput;
        private Control _hosting;
        private LineEdit _hostingCode;
        private Label _playersJoinedTemplate;

        // Create timer to cancel requests that take too long.
        private bool _requestInProgress = false;
        private float _requestTimeout = 5;
        private float _requestTimeoutTimer = 0;

        #region RequestTriggers

        private void HostNewGame()
        {
            // Webhook send new game request.
            LoadingStart();
            _requestHandler.HostGame();
        }

        private void JoinHostGame()
        {
            LoadingStart();
            var code = _enterCodeInput.Text;
            _requestHandler.JoinGame(code);
        }

        private void LeaveHostGame()
        {
            LoadingStart();
            string code = _gameManager.CurrentRoom;
            _requestHandler.LeaveGame(code);
        }

        private void StartNewGame()
        {
            // Webhook send new game request.
            LoadingStart();
            string code = _gameManager.CurrentRoom;
            _requestHandler.StartGame(code);
        }

        #endregion

        #region RequestResponses

        public void JoinGameSuccess(string code, string[] users)
        {
            HideAll();

            _hostingCode.Text = code;
            _hosting.Visible = true;

            _playersJoinedTemplate.Visible = true;
            _playersJoinedTemplate.Text = "Players joined: " + string.Join(", ", users);
            LoadingComplete();
        }

        public void JoinGameFailure()
        {
            // @todo Nothing calls this. Maybe add a request timeout?
            ShowFriendsPanel();
        }

        public void LeaveGameSuccess()
        {
            HideAll();

            ShowFriendsPanel();
        }

        public void LeaveGameFailure()
        {
            // @todo Nothing calls this. Maybe add a request timeout?
            LoadingComplete();
        }

        #endregion

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
            HostNewGame();
        }

        private void JoinHostingPanel()
        {
            JoinHostGame();
        }

        private void LoadingStart()
        {
            _requestInProgress = true;
            _loadingManager.ShowLoading();
        }

        private void LoadingComplete()
        {
            _requestTimeoutTimer = 0;
            _requestInProgress = false;
            _loadingManager.HideLoading();
        }

        private void LoadingDebug()
        {
            _loadingManager.ShowLoadingTimed(0.25f);
        }

        #region ButtonEvents

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
            JoinHostingPanel();
        }

        public void _OnButtonDown_Join_Back()
        {
            ShowFriendsPanel();
        }

        public void _OnButtonDown_Host_Start()
        {
            GD.Print("Start game.");
            StartNewGame();
        }

        public void _OnButtonDown_Host_Back()
        {
            LeaveHostGame();
        }

        #endregion

        private void EvaluateWebsocketResponses(WebSocket.WebSocketResponse response)
        {
            // If we have timed out, don't handle the response.
            // This is a sticky plaster solution to a problem that should be solved elsewhere. Disable during
            // development to catch bugs in the websocket response handler.
            // if (!_requestInProgress) return;
            _eventHandler.HandleResponse(response);
        }

        private void HandleRequestTimeout(float delta)
        {
            if (!_requestInProgress) return;

            _requestTimeoutTimer += delta;
            if (_requestTimeoutTimer >= _requestTimeout)
            {
                LoadingComplete();
                // @todo Show error message.
            }
        }

        #region GodotHooks

        public override void _Ready()
        {
            _loadingManager = GetNode<LoadingManager>("/root/LoadingManager");
            _webSocket = GetNode<WebSocket>("/root/WebSocket");
            _gameManager = GetNode<GameManager>("/root/GameManager");

            _startButtons = GetNode<Control>("StartButtons");
            _friendsButtons = GetNode<Control>("FriendsButtons");
            _enterCode = GetNode<Control>("EnterCode");
            _enterCodeInput = GetNode<LineEdit>("EnterCode/LineEdit");
            _hosting = GetNode<Control>("Hosting");
            _hostingCode = _hosting.GetNode<LineEdit>("Code");
            _playersJoinedTemplate = GetNode<Label>("Hosting/_PlayersJoined/_PlayerTemplate");
            CallDeferred(nameof(ShowHomePanel));

            _eventHandler = new WebSocketEventHandler(this, _gameManager, _loadingManager, _webSocket);
            _requestHandler = new WebSocketRequestHandler(this, _gameManager, _loadingManager, _webSocket);
        }

        public override void _Process(float delta)
        {
            HandleRequestTimeout(delta);
        }

        public override void _EnterTree()
        {
            WebSocket.OnResponseReceived += EvaluateWebsocketResponses;
        }

        public override void _ExitTree()
        {
            WebSocket.OnResponseReceived -= EvaluateWebsocketResponses;
        }

        #endregion

        public void StartGameSuccess(GameState gameState)
        {
            _gameManager.StartNewGame(gameState);
            GetTree().ChangeScene("res://Scenes/Game.tscn");
        }
    }
}
