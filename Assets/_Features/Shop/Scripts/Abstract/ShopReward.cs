namespace Quackery.Shops
{
    public abstract class ShopReward
    {
        public virtual ShopRewardType Type { get; private set; }

        public abstract int Price { get; }
        public abstract string Description { get; }
        public abstract bool IsSubscription { get; }
        public abstract void ApplyReward();
    }
}
