

using System;
using System.Collections.Generic;
using Quackery.Inventories;
using UnityEngine;


namespace Quackery.Shops
{
    public static class ShopServices
    {
        public static Func<WaitUntil> WaitUntilReady = () => new WaitUntil(() => true);
        public static Func<ShopData, List<Item>> GetAllItems = (showData) => new();
    }
}