using System.Collections.Generic;
using GameCore;
using GameCore.Players;
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

        private void ClickedDiscardDeck()
        {
            if (_gameManager.DrawCard(DeckType.Discard))
            {
                return;
            }

            _gameManager.DiscardCard();
        }

        private void ClickedDrawDeck()
        {
            if (!_gameManager.DrawCard(DeckType.Draw))
            {
                return;
            }
        }

        public void PlaceCard(GameCore.Card card)
        {
            _gameManager.PlaceCard(card);
        }

        public int GetHoldCard()
        {
            var currentPlayer = TurnManager.Instance.GetCurrentTurn();
            GameCore.Players.Player player = _gameManager.GetPlayer(currentPlayer);
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
            _gameManager = GameCore.GameManager.Instance;

            _gameManager.StartListening();
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
            _gameManager.StopListening();
            _gameManager = null;

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

        private static void FetchPlayers(List<GameCore.Players.Player> players)
        {
            IPlayerBrain brain1 = new AIBrain();
            players.Add(new GameCore.Players.Player(brain1, PlayerId.Player1));
            IPlayerBrain brain2 = new AIBrain();
            players.Add(new GameCore.Players.Player(brain2, PlayerId.Player2));
        }
    }
}
