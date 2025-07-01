using Microsoft.Unity.VisualStudio.Editor;
using Quackery.Inventories;
using UnityEngine;
using UnityEngine.Assertions;

namespace Quackery.Shops
{
    [CreateAssetMenu(fileName = "NewCardReward", menuName = "Quackery/Shop/New Card Reward")]
    public class NewCardReward : ShopReward
    {

        public override int Price => ItemData.SubscriptionCost;

        public override string Description => ItemData.ShortDescription;

        public override bool IsSubscription => true;

        public override string Title => ItemData.MasterText;
        ItemData _itemData;

        public ItemData ItemData
        {
            get
            {
                if (_itemData == null)
                    _itemData = InventoryServices.GetRandomItemData();
                return _itemData;
            }
        }

        public override void ApplyReward()
        {
            DeckServices.AddNewToDraw(ItemData, true);
        }
    }
}
