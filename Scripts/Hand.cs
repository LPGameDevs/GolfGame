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
            UpdateHoldCard(false);
        }

        public void UpdateNumber()
        {
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
            {
                GD.PrintErr(e.Message);
            }

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

        private void Refresh()
        {
            // @todo: Get the holding card from state and update showing it.
            UpdateNumber();
        }

        public override void _EnterTree()
        {
            GameCore.Players.Player.OnPlayerHoldCard += UpdateHoldCard;
            DeckManager.OnRefresh += Refresh;
        }

        public override void _ExitTree()
        {
            GameCore.Players.Player.OnPlayerHoldCard -= UpdateHoldCard;
            DeckManager.OnRefresh -= Refresh;
        }
    }
}
