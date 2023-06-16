using Godot;

namespace GolfGame
{
    public class LoadingManager : Node
    {
        private Control _loadingNode;

        private bool _isSpawned = false;
        private float _showTimer = 0;

        public override void _Ready()
        {
            var node = (PackedScene)ResourceLoader.Load("res://Prefabs/Loading.tscn");
            _loadingNode = (Control) node.Instance();
        }


        public override void _Process(float delta)
        {
            if (_showTimer <= 0)
            {
                return;
            }

            _showTimer -= delta;
            if (_showTimer <= 0)
            {
                HideLoading();
            }
        }

        private void SpawnLoading()
        {
            _isSpawned = true;
            GetTree().Root.AddChild(_loadingNode);
        }

        public void ShowLoading()
        {
            if (!_isSpawned)
            {
                SpawnLoading();
            }
            _loadingNode.Visible = true;
        }

        public void HideLoading()
        {
            if (!_isSpawned)
            {
                SpawnLoading();
            }
            _loadingNode.Visible = false;
        }

        public void ShowLoadingTimed(float time)
        {
            ShowLoading();
            _showTimer = time;
        }
    }
}
