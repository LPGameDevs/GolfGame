using System;
using GameCore;
using GameCore.Players;
using Godot;

namespace GolfGame
{
    public class Hand : Control
    {

        private GameManager _gm;
        private Label _number;

        public override void _Ready()
        {
            _gm = GetNode<GameManager>("/root/GameManager");

            _number = GetNode<Label>("NinePatchRect/MarginContainer/Number");
            UpdateNumber();
        }

        public void UpdateNumber()
        {
            if (_number == null)
            {
                UpdateHoldCard(false);
                return;
            }

            try
            {
                CardDto holdCard = _gm.GetHoldCard();
                if (holdCard.CardName != CardName.None)
                {
                    _number.Text = holdCard.ToString();
                    return;
                }
            }
            catch (PlayerNotFoundException e)
            { }

            // Something went wrong so just hide the number.
            UpdateHoldCard(false);
        }

        private void UpdateHoldCard(bool isHolding)
        {
            if (isHolding)
            {
                UpdateNumber();
                Visible = true;
            }
            else
            {
                Visible = false;
            }

        }

        public override void _EnterTree()
        {
            GameCore.Players.Player.OnPlayerHoldCard += UpdateHoldCard;
        }

        public override void _ExitTree()
        {
            GameCore.Players.Player.OnPlayerHoldCard -= UpdateHoldCard;
        }
    }
}
