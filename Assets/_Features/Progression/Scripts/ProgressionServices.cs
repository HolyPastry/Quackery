using System;
using UnityEngine;

namespace Quackery.Progression
{
    public static class ProgressionServices
    {
        public static Func<WaitUntil> WaitUntilReady = () => new WaitUntil(() => true);
        public static Func<int> GetLevel = () => 0;
        public static Action<int> SetLevel = level => { };

        public static Func<bool> HasLeveledUp = () => false;
        public static Action ConsumeLeveledUp = () => { };
    }
}
