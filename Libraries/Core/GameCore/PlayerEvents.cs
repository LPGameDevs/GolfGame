
using System;

namespace GameCore
{
    public class PlayerEvents
    {
        public static event Action OnPlayerViewCards;
        public static event Action OnPlayerDrawCard;
        public static event Action OnPlayerDiscardCard;
        public static event Action OnPlayerCallLastRound;
        public static event Action OnPlayerCompleteTurn;

        public static void ViewCards()
        {
            OnPlayerViewCards?.Invoke();
        }

        public static void DrawCard()
        {
            OnPlayerDrawCard?.Invoke();
        }

        public static void DiscardCard()
        {
            OnPlayerDiscardCard?.Invoke();
        }

        public static void CallLastRound()
        {
            OnPlayerCallLastRound?.Invoke();
        }

        public static void CompleteTurn()
        {
            OnPlayerCompleteTurn?.Invoke();
        }

        public void StartListening()
        {
            OnPlayerCallLastRound += CompleteTurn;
        }

        public void StopListening()
        {
            OnPlayerCallLastRound -= CompleteTurn;
        }

    }
}
