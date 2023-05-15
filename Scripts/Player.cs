using Godot;

namespace GolfGame
{
    public class Player : ColorRect
    {
        private GameCore.Players.Player _player;

        private Card _card1, _card2, _card3, _card4;
        private Card[] _cards;

        public override void _Ready()
        {
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
    }
}
