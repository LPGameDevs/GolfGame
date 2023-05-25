using GameCore;
using Godot;

namespace GolfGame
{
    public class Player : ColorRect
    {
        private GameCore.Players.Player _player;

        private Score _playerScore;
        private Card _card1, _card2, _card3, _card4;
        private Card[] _cards;

        private Color _inactiveColor;
        private Color _activeColor = new Color(1, 0, 0);

        public override void _Ready()
        {
            _inactiveColor = Color;

            _playerScore = GetNode<Score>("Score");

            _card1 = GetNode<Card>("MarginContainer/BoxContainer/Card1");
            _card2 = GetNode<Card>("MarginContainer/BoxContainer/Card2");
            _card3 = GetNode<Card>("MarginContainer/BoxContainer/Card3");
            _card4 = GetNode<Card>("MarginContainer/BoxContainer/Card4");

            _cards = new[]
            {
                _card1,
                _card2,
                _card3,
                _card4,
            };
        }


        public void SetPlayer(GameCore.Players.Player player)
        {
            _player = player;

            if (_playerScore != null)
            {
                _playerScore.SetPlayer(player.Id);
            }

            UpdateCards();
        }

        private void UpdateCards()
        {
            var cards = GetCards();
            for (int i = 0; i < cards.Length; i++)
            {
                _cards[i].SetCard(cards[i]);
            }
        }

        public GameCore.Card[] GetCards()
        {
            return _player.Cards;
        }

        private void OnNextPlayerTurn(PlayerId current, PlayerId next)
        {
            if (_player == null)
            {
                return;
            }

            if (current != _player.Id && next != _player.Id)
            {
                return;
            }

            if (current == _player.Id)
            {
                SetActive(false);
                // _player.EndTurn();
            }
            else
            {
                SetActive(true);
                // _player.StartTurn();
            }
        }

        private void SetActive(bool isActive)
        {
            if (isActive)
            {
                Color = _activeColor;
            }
            else
            {
                Color = _inactiveColor;
            }
        }

        public override void _EnterTree()
        {
            TurnManager.OnTransitionPlayerTurn += OnNextPlayerTurn;
        }

        public override void _ExitTree()
        {
            TurnManager.OnTransitionPlayerTurn -= OnNextPlayerTurn;
        }
    }
}
