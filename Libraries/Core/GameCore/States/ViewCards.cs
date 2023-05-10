
using System;

namespace GameCore.States
{
    public class ViewCards : IState
    {
        public static event Action OnExit;

        public void Enter()
        {
            PlayerEvents.OnPlayerViewCards += Exit;
        }

        public void Exit()
        {
            PlayerEvents.OnPlayerViewCards -= Exit;
            OnExit?.Invoke();
        }
    }
}
