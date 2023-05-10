
using System;

namespace GameCore.States
{
    public class DrawCard : IState
    {
        public static event Action OnExit;

        public void Enter()
        {
            PlayerEvents.OnPlayerDrawCard += Exit;
        }

        public void Exit()
        {
            PlayerEvents.OnPlayerDrawCard -= Exit;
            OnExit?.Invoke();
        }
    }
}
