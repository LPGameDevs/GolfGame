using System;
using GameCore;
using Godot;

public class Discard : Control
{
    public static event Action OnDiscardClicked;

    private Label _number;

    public override void _Ready()
    {
        _number = GetNode<Label>("NinePatchRect/MarginContainer/Number");
        UpdateNumber();
    }

    public void UpdateNumber()
    {
        if (_number == null)
        {
            return;
        }

        try
        {
            int number = DeckManager.Instance.GetDiscardTopCard();
            _number.Text = number.ToString();
        }
        catch (DrawFromEmptyDiscardException e)
        {
            _number.Text = "NA";
        }
    }

    private void DrawNewCard()
    {
        int newNumber = DeckManager.Instance.DrawCard();
        DeckManager.Instance.DiscardCard(newNumber);
        UpdateNumber();
    }


    public void _OnButtonDown()
    {
        OnDiscardClicked?.Invoke();
        UpdateNumber();
    }

    public override void _EnterTree()
    {
        DeckManager.OnCardDiscarded += UpdateNumber;
    }

    public override void _ExitTree()
    {
        DeckManager.OnCardDiscarded -= UpdateNumber;
    }
}
