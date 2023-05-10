
using System;

namespace GameCore.States
{
    public class CompleteTurn : IState
    {
        public static event Action OnExit;

        public void Enter()
        {
            PlayerEvents.OnPlayerCompleteTurn += Exit;
        }

        public void Exit()
        {
            PlayerEvents.OnPlayerCompleteTurn -= Exit;
            OnExit?.Invoke();
        }
    }
}
