using System.Linq;
using GameCore.States;
using Godot;

namespace GolfGame
{
    public class Card : Control
    {
        private GameManager _gm;

        private GameCore.Card _card;

        [Export] public Color SelectedColor { get; set; } = new Color(1, 0, 0);

        private bool _active;

        private Label _number;
        private ColorRect _colorRect;
        private Color _defaultColor;

        public override void _Ready()
        {
            _gm = GetNode<GameManager>("/root/GameManager");

            _number = GetNode<Label>("NinePatchRect/MarginContainer/Control/Number");
            _colorRect = GetNode<ColorRect>("Color");
            _defaultColor = _colorRect.Color;

            _colorRect.SetSize(RectSize);

            // int newNumber = (int) GD.RandRange(1, 14);
            // UpdateNumber(newNumber);
        }

        public void UpdateNumber()
        {
            _number.Text = _card.CardData.CardName.ToString();
        }

        public void _OnButtonDown()
        {
            _gm.PlaceCard(_card);
            UpdateNumber();
            // ToggleActive();
        }

        private void ToggleActive()
        {
            if (!CanToggleActive())
            {
                return;
            }

            _active = !_active;
            UpdateColor();
        }

        private bool CanToggleActive()
        {
            string[] allowedStates =
            {
                nameof(DiscardCard),
                nameof(ViewCards)
            };

            if (allowedStates.Contains(_gm.GetCurrentState()))
            {
                return true;
            }

            return false;
        }

        private void UpdateColor()
        {
            if (_active)
            {
                _colorRect.Color = SelectedColor;
                GD.Print("Card is active");
            }
            else
            {
                _colorRect.Color = _defaultColor;
                GD.Print("Card is not active");
            }
        }

        private void OnExitViewCards()
        {
            _active = false;
            UpdateColor();
        }

        public override void _EnterTree()
        {
            ViewCards.OnExit += OnExitViewCards;
        }

        public override void _ExitTree()
        {
            ViewCards.OnExit -= OnExitViewCards;
        }

        public void SetCard(GameCore.Card card)
        {
            _card = card;
            UpdateNumber();
        }
    }
}
