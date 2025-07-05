using UnityEngine;

namespace Quackery.GameStats
{
    public static class GameStatsServices
    {
        public static System.Func<WaitUntil> WaitUntilReady = () => new WaitUntil(() => true);
        public static System.Func<CardStats, int> NumMatchingCard = (stats) => 0;

    }
}
