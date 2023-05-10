using System;

namespace GameCore
{
    public abstract class StateMachine : IStateMachine
    {
        public static event Action<string,string> OnTransitionState;
        public static event Action OnTurnFinished;

        protected IState _currentState = null;

        public string GetCurrentState()
        {
            return _currentState.GetType().Name;
        }

        protected void TransitionTo(IState state)
        {
            if (_currentState != null)
            {
                OnTransitionState?.Invoke(_currentState.GetType().Name, state.GetType().Name);
            }

            _currentState = state;
            _currentState.Enter();
        }

        public abstract void StartNewGame();

        public abstract void StartNewMatch();

        public abstract void StartNewTurn();

        public abstract void StartListening();

        public abstract void StopListening();
    }

    public interface IStateMachine
    {
        string GetCurrentState();
        void StartNewGame();
        void StartNewMatch();
        void StartNewTurn();

        void StartListening();
        void StopListening();
    }

    public interface IState
    {
        void Enter();

        void Exit();
    }
}
