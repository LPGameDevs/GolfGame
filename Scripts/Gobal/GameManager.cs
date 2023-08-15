using System.Collections.Generic;
using GameCore;
using GameCore.Players;
using GameCore.States;
using Godot;

namespace GolfGame
{
    public class GameManager : Node
    {
        GameCore.GameManager _coreGameManager = null;
        TurnManager _turnManager = null;

        private GameState _gameState;

        private bool _debug = true;

        public void StartNewGame(GameState gameState)
        {
            _gameState = gameState;
            _coreGameManager.StartNewGame(_gameState);
        }

        public string GetCurrentState()
        {
            return _coreGameManager.GetCurrentState();
        }

        private void ClickedDiscardDeck()
        {
            if (_coreGameManager.DrawCard(DeckType.Discard))
            {
                return;
            }

            _coreGameManager.DiscardCard();
        }

        private void ClickedDrawDeck()
        {
            if (!_coreGameManager.DrawCard(DeckType.Draw))
            {
                return;
            }
        }

        public void PlaceCard(GameCore.Card card)
        {
            _coreGameManager.PlaceCard(card);
        }

        public CardDto GetHoldCard()
        {
            var currentPlayer = TurnManager.Instance.GetCurrentTurn();
            GameCore.Players.Player player = _coreGameManager.GetPlayer(currentPlayer);
            return player.GetHoldingCard();
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
                TurnManager.Instance.NextPlayerStartTurn();
            }
            else if (GetCurrentState() == nameof(CompleteTurn))
            {
                // @fixme This should be replaced by multiplayer code.
                PlayerEvents.CompleteTurn();
            }

            GD.Print("Continue button pressed.");
        }

        public override void _EnterTree()
        {
            _turnManager = TurnManager.Instance;
            _coreGameManager = GameCore.GameManager.Instance;

            _coreGameManager.StartListening();
            _turnManager.StartListening();

            if (_debug)
            {
                StateMachine.OnTransitionState += LogStateTransition;
                TurnManager.OnTransitionPlayerTurn += LogPlayerTransition;
            }

            PlayerManager.OnFetchPlayers += FetchPlayers;
            Buttons.OnContinueButtonPressed += ButtonContinue;
            Deck.OnDeckClicked += ClickedDrawDeck;
            Discard.OnDiscardClicked += ClickedDiscardDeck;
        }

        public override void _ExitTree()
        {
            _coreGameManager.StopListening();
            _coreGameManager = null;

            _turnManager.StopListening();
            _turnManager = null;

            if (_debug)
            {
                StateMachine.OnTransitionState -= LogStateTransition;
                TurnManager.OnTransitionPlayerTurn -= LogPlayerTransition;
            }

            PlayerManager.OnFetchPlayers -= FetchPlayers;
            Buttons.OnContinueButtonPressed -= ButtonContinue;
            Deck.OnDeckClicked -= ClickedDrawDeck;
            Discard.OnDiscardClicked -= ClickedDiscardDeck;
        }

        private static void LogStateTransition(string arg1, string arg2)
        {
            GD.Print($"Transitioning from {arg1} to {arg2}");
        }

        private static void LogPlayerTransition(PlayerId arg1, PlayerId arg2)
        {
            GD.Print($"Moving turn from {arg1.ToString()} to {arg2.ToString()}");
        }

        private void FetchPlayers(List<GameCore.Players.Player> players)
        {
            int i = 1;
            foreach (var user in _gameState.users)
            {
                IPlayerBrain brain = new AIBrain();
                players.Add(new GameCore.Players.Player(brain, (PlayerId) i));

                i++;
            }
        }

        #region RoomStuff

        public string CurrentUser = null;
        public string CurrentRoom = null;

        public void JoinRoom(string roomName)
        {
            CurrentRoom = roomName;
        }

        public void LeaveRoom()
        {
            CurrentRoom = null;
        }

        #endregion
    }
}
