
using System;
using UnityEngine;

namespace Quackery.Followers
{
    public class FollowerServices
    {
        public static Func<WaitUntil> WaitUntilReady = () => new WaitUntil(() => true);
        public static Func<int> GetNumberOfFollowers = () => 0;
        public static Action<int> ModifyFollowers = delegate { };
        public static Action<int> SetNumberOfFollowers = delegate { };

        public static Func<int, int> RewardFollowers = (SuccessRating) => 0;

        public static Func<FollowerLevel> GetCurrentLevel = () => default;
        public static Func<int> GetNumFollowersToNextLevel = () => 0;

        internal static Func<FollowerLevel> GetNextLevel = () => default;

    }
}
