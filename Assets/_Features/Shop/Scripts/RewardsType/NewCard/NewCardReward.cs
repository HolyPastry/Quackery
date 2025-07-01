using Microsoft.Unity.VisualStudio.Editor;
using Quackery.Inventories;
using UnityEngine;
using UnityEngine.Assertions;

namespace Quackery.Shops
{
    public class NewCardReward : ShopReward
    {

        public override int Price => ItemData.SubscriptionCost;

        public override string Description => ItemData.ShortDescription;

        public override bool IsSubscription => true;

        public override string Title => ItemData.MasterText;

        public ItemData ItemData;

        public override void ApplyReward()
        {
            DeckServices.AddNewToDraw(ItemData, true);
        }
    }
}
