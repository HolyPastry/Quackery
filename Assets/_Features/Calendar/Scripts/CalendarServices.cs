using System;
using UnityEngine;

namespace Quackery
{
    public static class CalendarServices
    {
        public static Func<WaitUntil> WaitUntilReady = () => new WaitUntil(() => true);
        public static Action GoToNextDay = delegate { };

        internal static Func<int> Today = () => 0;

    }
}
