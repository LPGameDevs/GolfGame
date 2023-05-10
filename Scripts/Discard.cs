using Godot;

public class Discard : Control
{
    private Label _number;

    public override void _Ready()
    {
        _number = GetNode<Label>("NinePatchRect/MarginContainer/Number");
    }

    public void UpdateNumber(int number)
    {
        _number.Text = number.ToString();
    }

    private void DrawNewCard()
    {
        int newNumber = (int) GD.RandRange(1, 14);
        UpdateNumber(newNumber);
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
