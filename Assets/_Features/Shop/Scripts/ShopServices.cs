using System;
using System.Collections;
using System.Collections.Generic;
using Quackery.Shops;

namespace Quackery
{
    public static class ShopServices
    {
        public static Func<int, List<ShopReward>> GetRewards = (amount) => new();
        //public static Action<ShopReward> ApplyReward = (data) => { };
    }
}
