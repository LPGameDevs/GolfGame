using Godot;

namespace GameCore
{
    public class GameManager
    {
        // Track the current match.
        // Track the current player.

        private bool _isLastRound = false;
        private bool _isGameOver = false;
        public bool IsLastRound => _isLastRound;
        public bool IsGameOver => _isGameOver;

        IStateMachine _stateMachine;

        public void SetStateMachine(IStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void StartNewGame()
        {
            _stateMachine.StartNewGame();
            StartNewMatch();
        }

        public void StartNewMatch()
        {
            _isLastRound = false;
            _stateMachine.StartNewMatch();
        }

        public void StartNewTurn()
        {
            if (!TurnManager.Instance.IsCurrentPlayerTurn)
            {
                return;
            }
            _stateMachine.StartNewTurn();
        }

        private void StartLastRound()
        {
            _isLastRound = true;
        }

        public string GetCurrentState()
        {
            GD.Print(_stateMachine.GetCurrentState());
            return _stateMachine.GetCurrentState();
        }

        public void StartListening()
        {
            _stateMachine.StartListening();
            PlayerEvents.OnPlayerCallLastRound += StartLastRound;
        }

        public void StopListening()
        {
            _stateMachine.StopListening();
            PlayerEvents.OnPlayerCallLastRound -= StartLastRound;
        }

        #region Singleton

        private static GameManager _instance;

        private GameManager()
        {
        }

        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameManager();
                }

                return _instance;
            }
        }

        #endregion

        public void GameOver()
        {
            _isGameOver = true;
        }
    }

}
