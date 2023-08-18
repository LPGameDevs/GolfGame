using System.Collections.Generic;
using GameCore;
using GameCore.Players;
using GameCore.States;
using Godot;
using GolfGame.Helpers;

namespace GolfGame
{
    public class GameManager : Node
    {
        GameCore.GameManager _coreGameManager = null;
        TurnManager _turnManager = null;

        WebSocket _webSocket;

        WebSocketGameRequestHandler _requestHandler;
        WebSocketGameEventHandler _eventHandler;

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
                _requestHandler.DrawCardDiscard(_gameState.id);
                return;
            }

            if (_coreGameManager.DiscardCard())
            {
                _requestHandler.DiscardHandCard(_gameState.id);
                return;
            }
        }

        private void ClickedDrawDeck()
        {
            if (_coreGameManager.DrawCard(DeckType.Draw))
            {
                _requestHandler.DrawCardDeck(_gameState.id);
                return;
            }
        }

        public void PlaceCard(GameCore.Cards.Card card)
        {
            if (_coreGameManager.PlaceCard(card))
            {
                _requestHandler.PlaceHandCard(_gameState.id, card.Index);
                return;
            }
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

        private void EvaluateWebsocketResponses(WebSocket.WebSocketResponse response)
        {
            _eventHandler.HandleResponse(response);
        }

        public override void _Ready()
        {
            _webSocket = GetNode<WebSocket>("/root/WebSocket");
            _requestHandler = new WebSocketGameRequestHandler(this, _webSocket);
            _eventHandler = new WebSocketGameEventHandler(this, _webSocket);

            GD.Print("GameManager ready.");

            // @todo This is just for testing.
#if DEPLOY_ENVIRONMENT
            GD.Print("DEPLOY_ENVIRONMENT");
#endif
#if ENV_PROD_SUCCESS
            GD.Print("ENV_PROD_SUCCESS");
#endif
        }

        public override void _EnterTree()
        {
            _turnManager = TurnManager.Instance;
            _coreGameManager = GameCore.GameManager.Instance;

            _turnManager.StartListening();

            if (_debug)
            {
                StateMachine.OnTransitionState += LogStateTransition;
                TurnManager.OnTransitionPlayerTurn += LogPlayerTransition;
            }

            Buttons.OnContinueButtonPressed += ButtonContinue;
            Deck.OnDeckClicked += ClickedDrawDeck;
            Discard.OnDiscardClicked += ClickedDiscardDeck;
            WebSocket.OnResponseReceived += EvaluateWebsocketResponses;
        }

        public override void _ExitTree()
        {
            _coreGameManager = null;

            _turnManager.StopListening();
            _turnManager = null;

            if (_debug)
            {
                StateMachine.OnTransitionState -= LogStateTransition;
                TurnManager.OnTransitionPlayerTurn -= LogPlayerTransition;
            }

            Buttons.OnContinueButtonPressed -= ButtonContinue;
            Deck.OnDeckClicked -= ClickedDrawDeck;
            Discard.OnDiscardClicked -= ClickedDiscardDeck;
            WebSocket.OnResponseReceived -= EvaluateWebsocketResponses;
        }

        private static void LogStateTransition(string arg1, string arg2)
        {
            GD.Print($"Transitioning from {arg1} to {arg2}");
        }

        private static void LogPlayerTransition(PlayerId arg1, PlayerId arg2)
        {
            GD.Print($"Moving turn from {arg1.ToString()} to {arg2.ToString()}");
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

        public void ConfirmUserAction(string uid, WebSocketResponseType webSocketResponseType, GameState gameState)
        {
            UpdateGameState(gameState);
            if (uid != CurrentUser)
            {
                UpdateGameFromState(gameState);
                return;
            }

            switch (webSocketResponseType)
            {
                case WebSocketResponseType.CardDrawn:
                    PlayerEvents.DrawCard();
                    break;
                case WebSocketResponseType.CardPlaced:
                    PlayerEvents.DiscardCard();
                    break;
                case WebSocketResponseType.TurnCompleted:
                    PlayerEvents.CompleteTurn();
                    break;
                case WebSocketResponseType.PlayerKnocked:
                    PlayerEvents.CallLastRound();
                    break;
            }
        }

        private void UpdateGameState(GameState gameState)
        {
            _gameState.discard = gameState.discard;
            _gameState.deck = gameState.deck;
            _gameState.hands = gameState.hands;
        }

        private void UpdateGameFromState(GameState gameState)
        {
            _coreGameManager.UpdateGameFromState(gameState);
        }
    }
}
