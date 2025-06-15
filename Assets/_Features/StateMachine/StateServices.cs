using System;

namespace Quackery.StateMachines
{
    public static class StateServices
    {
        public static Func<State> GetCurrentState = () => null;
    }
}
