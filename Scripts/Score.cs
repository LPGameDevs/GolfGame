using GameCore;
using Godot;

namespace GolfGame
{
    public class Score : ColorRect
    {
        private ScoreManager _scoreManager;

        private PlayerId _playerId = PlayerId.NoPlayer;

        private Label _scoreLabel;

        public override void _Ready()
        {
            _scoreManager = GetNode<ScoreManager>("/root/ScoreManager");
            _scoreLabel = GetNode<Label>("Number");
            UpdateScore(false);
        }

        private void UpdateScore(bool _)
        {
            if (_playerId == PlayerId.NoPlayer)
            {
                return;
            }

            int score = _scoreManager.GetPlayerScore(_playerId);
            _scoreLabel.Text = score.ToString();
        }

        public void SetPlayer(PlayerId playerId)
        {
            _playerId = playerId;
            UpdateScore(false);
        }

        public override void _EnterTree()
        {
            GameCore.Players.Player.OnPlayerHoldCard += UpdateScore;
        }

        public override void _ExitTree()
        {
            GameCore.Players.Player.OnPlayerHoldCard -= UpdateScore;
        }
    }
}
