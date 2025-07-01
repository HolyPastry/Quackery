using UnityEngine;

namespace Quackery.Shops
{
    public abstract class ShopReward : ScriptableObject
    {
        public abstract string Title { get; }
        public abstract int Price { get; }
        public abstract string Description { get; }
        public abstract bool IsSubscription { get; }
        public ShopPost PostPrefab;
        public abstract void ApplyReward();
    }
}
