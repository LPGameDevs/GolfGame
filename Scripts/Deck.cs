using System;
using Godot;
using GolfGame;

public class Deck : Control
{
    public static event Action OnDeckClicked;

    private GameManager _gm;

    private Label _number;

    public override void _Ready()
    {
        _gm = GetNode<GameManager>("/root/GameManager");

        _number = GetNode<Label>("NinePatchRect/MarginContainer/Control/Number");
        _number.Visible = false;
    }

    public void _OnButtonDown()
    {
        OnDeckClicked?.Invoke();
    }

    public void UpdateHoldCard(bool isHolding)
    {
        if (isHolding)
        {
            int card = _gm.GetHoldCard();
            _number.Text = card.ToString();
            _number.Visible = true;

        }
        else
        {
            _number.Visible = false;
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
