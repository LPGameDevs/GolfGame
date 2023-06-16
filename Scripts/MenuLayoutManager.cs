using Godot;

namespace GolfGame
{
    public class MenuLayoutManager : Control
    {
        private Control _startButtons;
        private Control _friendsButtons;
        private Control _enterCode;
        private Control _hosting;


        public override void _Ready()
        {
            _startButtons = GetNode<Control>("StartButtons");
            _friendsButtons = GetNode<Control>("FriendsButtons");
            _enterCode = GetNode<Control>("EnterCode");
            _hosting = GetNode<Control>("Hosting");
            ShowHomePanel();
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
        }

        private void ShowFriendsPanel()
        {
            HideAll();
            _friendsButtons.Visible = true;
        }

        private void ShowCodePanel()
        {
            HideAll();
            _enterCode.Visible = true;
        }

        private void ShowHostingPanel()
        {
            HideAll();
            _hosting.Visible = true;
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
