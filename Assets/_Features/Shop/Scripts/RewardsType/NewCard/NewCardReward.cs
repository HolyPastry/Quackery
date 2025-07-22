using System;
using Quackery.Inventories;

namespace Quackery.Shops
{

    public class NewCardReward : ShopReward
    {

        public override int Price => ItemData.SubscriptionCost;

        public override string Description => ItemData.ShopDescription;

        public override bool IsSubscription => true;

        public override string Title => ItemData.MasterText;

        public ItemData ItemData { get; private set; }

        public NewCardReward(ItemData itemData)
        {
            ItemData = itemData;
        }


        public override void ApplyReward()
        {
            DeckServices.AddNew(ItemData,
                            Decks.EnumCardPile.Discard,
                            EnumPlacement.OnTop,
                            EnumLifetime.Permanent);
        }
    }
}
