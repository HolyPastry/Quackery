using Quackery.Inventories;
using UnityEditor.VersionControl;
using UnityEngine.Assertions;

namespace Quackery.Shops
{
    public class AddCardReward : ShopReward
    {
        public override ShopRewardType Type => ShopRewardType.AddCard;

        public override int Price => ItemData.SubscriptionCost;

        public override string Description => ItemData.ShortDescription;

        public override bool IsSubscription => true;

        public ItemData ItemData;

        public AddCardReward()
        {
            ItemData = InventoryServices.GetRandomItemData();
            Assert.IsNotNull(ItemData, "ItemData cannot be null for AddCardReward.");
        }

        public override void ApplyReward()
        {
            DeckServices.AddToDrawPile(new() { ItemData });
        }
    }
}
