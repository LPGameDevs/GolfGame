using Godot;
using System;

public class Deck : Control
{
    public static event Action OnDeckClicked;

    public void _OnButtonDown()
    {
        OnDeckClicked?.Invoke();
    }
}
