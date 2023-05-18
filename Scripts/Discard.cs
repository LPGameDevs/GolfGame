using GameCore;
using Godot;

public class Discard : Control
{
    private Label _number;

    public override void _Ready()
    {
        _number = GetNode<Label>("NinePatchRect/MarginContainer/Number");
        UpdateNumber();
    }

    public void UpdateNumber()
    {
        int number = DeckManager.Instance.GetDiscardTopCard();
        _number.Text = number.ToString();
    }

    private void DrawNewCard()
    {
        int newNumber = DeckManager.Instance.DrawCard();
        DeckManager.Instance.DiscardCard(newNumber);
        UpdateNumber();
    }

    public override void _EnterTree()
    {
        Deck.OnDeckClicked += DrawNewCard;
    }

    public override void _ExitTree()
    {
        Deck.OnDeckClicked -= DrawNewCard;
    }
}
