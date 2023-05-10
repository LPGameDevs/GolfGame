
using System;

namespace GameCore.States
{
    public class DiscardCard : IState
    {
        public static Action OnExit;

        public void Enter()
        {
            PlayerEvents.OnPlayerDiscardCard += Exit;
        }

        public void Exit()
        {
            PlayerEvents.OnPlayerDiscardCard -= Exit;
            OnExit?.Invoke();
        }
    }
}
