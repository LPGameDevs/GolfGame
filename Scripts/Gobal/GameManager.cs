using GameCore;
using GameCore.States;
using Godot;

namespace GolfGame
{
    public class GameManager : Node
    {
        GameCore.GameManager _gameManager = null;
        TurnManager _turnManager = null;

        private bool _debug = true;

        public override void _Ready()
        {
            _gameManager.StartNewGame();
        }

        public string GetCurrentState()
        {
            return _gameManager.GetCurrentState();
        }

        public void ButtonContinue()
        {
            if (GetCurrentState() == nameof(ViewCards))
            {
                PlayerEvents.ViewCards();
            }
            else if (GetCurrentState() == nameof(Waiting))
            {
                // @fixme This should be replaced by multiplayer code.
                _gameManager.StartNewTurn();
            }

            GD.Print("Continue button pressed.");
        }

        public override void _EnterTree()
        {
            StateMachine stateMachine = new GolfStateMachine();
            _turnManager = TurnManager.Instance;
            _gameManager = GameCore.GameManager.Instance;
            _gameManager.SetStateMachine(stateMachine);

            _gameManager.StartListening();
            _turnManager.StartListening();

            if (_debug)
            {
                StateMachine.OnTransitionState += LogStateTransition;
                TurnManager.OnTransitionPlayerTurn += LogPlayerTransition;
            }

            Buttons.OnContinueButtonPressed += ButtonContinue;
        }

        public override void _ExitTree()
        {
            _gameManager.StopListening();
            _gameManager = null;

            _turnManager.StopListening();
            _turnManager = null;

            if (_debug)
            {
                StateMachine.OnTransitionState -= LogStateTransition;
                TurnManager.OnTransitionPlayerTurn -= LogPlayerTransition;
            }

            Buttons.OnContinueButtonPressed -= ButtonContinue;
        }

        private static void LogStateTransition(string arg1, string arg2)
        {
            GD.Print($"Transitioning from {arg1} to {arg2}");
        }

        private static void LogPlayerTransition(PlayerId arg1, PlayerId arg2)
        {
            GD.Print($"Moving turn from {arg1.ToString()} to {arg2.ToString()}");
        }
    }
}
