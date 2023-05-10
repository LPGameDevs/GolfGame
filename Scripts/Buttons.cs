using System;
using Godot;

namespace GolfGame
{
    public class Buttons : Control
    {
        public static event Action OnContinueButtonPressed;

        private Button _continueButton;

        public override void _Ready()
        {
            _continueButton = GetNode<Button>("ContinueButton");
        }

        public void _OnContinueButtonDown()
        {
            OnContinueButtonPressed?.Invoke();
        }
    }
}
