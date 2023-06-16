using Godot;

namespace GolfGame
{
    public class MenuLayoutManager : Control
    {
        LoadingManager _loadingManager;

        private Control _startButtons;
        private Control _friendsButtons;
        private Control _enterCode;
        private Control _hosting;


        public override void _Ready()
        {
            _loadingManager = GetNode<LoadingManager>("/root/LoadingManager");

            _startButtons = GetNode<Control>("StartButtons");
            _friendsButtons = GetNode<Control>("FriendsButtons");
            _enterCode = GetNode<Control>("EnterCode");
            _hosting = GetNode<Control>("Hosting");
            CallDeferred(nameof(ShowHomePanel));
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
            _hosting.Visible = true;
            LoadingDebug();
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

    }
}
