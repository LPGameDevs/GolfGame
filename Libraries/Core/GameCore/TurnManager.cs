using System;

namespace GameCore
{
    public enum PlayerId
    {
        Player1 = 1,
        Player2 = 2,
        Player3 = 3,
        Player4 = 4
    }

    public class TurnManager
    {
        public static event Action<PlayerId,PlayerId> OnTransitionPlayerTurn;


        private PlayerId _currentPlayer;
        private PlayerId _player;

        public bool IsCurrentPlayerTurn => _currentPlayer == _player;

        public void SetPlayer(PlayerId player)
        {
            _player = player;
            _currentPlayer = player;
        }

        // This should probably be private...
        public void NextPlayerStartTurn()
        {
            PlayerId previousPlayer = _currentPlayer;
            switch (_currentPlayer)
            {
                case PlayerId.Player1:
                    _currentPlayer = PlayerId.Player2;
                    break;
                case PlayerId.Player2:
                    _currentPlayer = PlayerId.Player3;
                    break;
                case PlayerId.Player3:
                    _currentPlayer = PlayerId.Player4;
                    break;
                case PlayerId.Player4:
                    _currentPlayer = PlayerId.Player1;
                    break;
            }

            OnTransitionPlayerTurn?.Invoke(previousPlayer, _currentPlayer);
            GameManager.Instance.StartNewTurn();
        }

        public void StartListening()
        {
            PlayerEvents.OnPlayerCompleteTurn += NextPlayerStartTurn;
        }

        public void StopListening()
        {
            PlayerEvents.OnPlayerCompleteTurn -= NextPlayerStartTurn;
        }

        #region Singleton

        private static TurnManager _instance;

        private TurnManager()
        {
        }

        public static TurnManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TurnManager();
                }

                return _instance;
            }
        }

        #endregion

    }
}