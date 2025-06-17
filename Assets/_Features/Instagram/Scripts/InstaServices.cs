using System;
using System.Collections;
using System.Collections.Generic;

namespace Quackery
{
    public static class InstaServices
    {
        public static Func<int, List<InstaReward>> GetRewards = (amount) => new();
        public static Action<InstaReward> ApplyReward = (data) => { };
    }
}
