using System;
using UnityEngine;


namespace Quackery.Ratings
{
    public static class RatingServices
    {
        public static Func<WaitUntil> WaitUntilReady = delegate
        {
            return new WaitUntil(() => true);
        };
        public static Action<int> Modify = delegate { };
        public static Func<int> GetRating = delegate { return 0; };
        public static Func<int> GetCardBonus = delegate { return 0; };
    }
}
