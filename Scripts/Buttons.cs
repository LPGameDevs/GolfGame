using System;
using Godot;

namespace GolfGame
{
    public class Buttons : Control
    {
        public static event Action OnContinueButtonPressed;

        private GameManager _gm;

        private Label _currentState;
        private Button _continueButton;

        public override void _Ready()
        {
            _gm = GetNode<GameManager>("/root/GameManager");

            _currentState = GetNode<Label>("CurrentState");
            _continueButton = GetNode<Button>("ContinueButton");
        }

        public override void _PhysicsProcess(float delta)
        {
            _currentState.Text = _gm.GetCurrentState();
        }

        public void _OnContinueButtonDown()
        {
            OnContinueButtonPressed?.Invoke();
        }
    }
}
