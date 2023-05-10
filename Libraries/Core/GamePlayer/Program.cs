
using System;
using GameCore;
using GameCore.States;

namespace GamePlayer
{
    public class Program
    {

        static void Main(string[] args)
        {
            // Setup
            SetupGame();

            PlayGame();

            EndGame();
        }

        private static void SetupGame()
        {
            Console.WriteLine("Setup Game");
            GolfStateMachine.OnTransitionState += LogStateTransition;
            TurnManager.OnTransitionPlayerTurn += LogPlayerTransition;


            IStateMachine stateMachine = new GolfStateMachine();
            GameManager.Instance.SetStateMachine(stateMachine);
            GameManager.Instance.StartListening();

            TurnManager.Instance.SetPlayer(PlayerId.Player1);
            TurnManager.Instance.StartListening();
        }

        private static void PlayGame()
        {
            Console.WriteLine("Play Game");

            GameManager.Instance.StartNewGame();

            while (!GameManager.Instance.IsGameOver)
            {
                Console.Write(".");

                if (GameManager.Instance.GetCurrentState() == nameof(ViewCards))
                {
                    Console.WriteLine("View Cards");
                    PlayerEvents.ViewCards();
                }
                else if (GameManager.Instance.GetCurrentState() == nameof(DrawCard))
                {
                    Console.WriteLine("Draw Card");
                    PlayerEvents.DrawCard();
                }
                else if (GameManager.Instance.GetCurrentState() == nameof(DiscardCard))
                {
                    Console.WriteLine("Discard Card");
                    PlayerEvents.DiscardCard();
                }
                else if (GameManager.Instance.GetCurrentState() == nameof(CompleteTurn))
                {
                    if (!GameManager.Instance.IsLastRound)
                    {
                        Console.WriteLine("Call Last Round");
                        PlayerEvents.CallLastRound();
                    }
                    else
                    {
                        Console.WriteLine("Complete Turn");
                        PlayerEvents.CompleteTurn();
                    }
                }
                else if (GameManager.Instance.GetCurrentState() == nameof(Waiting) && GameManager.Instance.IsLastRound && TurnManager.Instance.IsCurrentPlayerTurn)
                {
                    GameManager.Instance.GameOver();
                }
                else if (GameManager.Instance.GetCurrentState() == nameof(Waiting))
                {
                    TurnManager.Instance.NextPlayerStartTurn();
                }
                else
                {
                    GameManager.Instance.StartNewTurn();
                }
            }

        }

        private static void EndGame()
        {
            Console.WriteLine("End Game");
            GolfStateMachine.OnTransitionState -= LogStateTransition;
            TurnManager.OnTransitionPlayerTurn += LogPlayerTransition;

            GameManager.Instance.StopListening();
            TurnManager.Instance.StopListening();
        }

        private static void LogStateTransition(string arg1, string arg2)
        {
            Console.WriteLine($"Transitioning from {arg1} to {arg2}");
        }
        private static void LogPlayerTransition(PlayerId arg1, PlayerId arg2)
        {
            Console.WriteLine($"Moving turn from {arg1.ToString()} to {arg2.ToString()}");
        }
    }
}
