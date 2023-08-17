using System;
using GameCore;
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

        private void UpdateCurrentState(string _, string state)
        {
            _currentState.Text = state;
        }

        public void _OnContinueButtonDown()
        {
            OnContinueButtonPressed?.Invoke();
        }

        public override void _EnterTree()
        {
            StateMachine.OnTransitionState += UpdateCurrentState;
        }

        public override void _ExitTree()
        {
            StateMachine.OnTransitionState -= UpdateCurrentState;
        }
    }
}
