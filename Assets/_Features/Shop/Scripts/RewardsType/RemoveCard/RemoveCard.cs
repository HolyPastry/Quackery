using Quackery.Inventories;
using Quackery.Shops;
using UnityEngine;

namespace Quackery
{

    public class RemoveCardReward : ShopReward
    {
        public override int Price => _removeCardPrice;
        private int _removeCardPrice;

        public override string Description => "Remove a card from your deck for " + _removeCardPrice + " coins.";
        public override bool IsSubscription => false;

        public override string Title => throw new System.NotImplementedException();

        //Added later on when the player selects a card to remove
        public Item ItemToRemove;

        public RemoveCardReward(int removeCardPrice)
        {
            _removeCardPrice = removeCardPrice;
        }

        public override void ApplyReward()
        {
            // if (ItemToRemove != null)
            //     DeckServices.DestroyCard(ItemToRemove);
        }
    }
}
