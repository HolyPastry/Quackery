using Quackery.Inventories;
using Quackery.Shops;
using UnityEngine;

namespace Quackery
{

    public class RemoveCard : ShopReward
    {
        public override int Price => _removeCardPrice;
        private int _removeCardPrice;


        public override ShopRewardType Type => ShopRewardType.RemoveCard;

        public override string Description => "Remove a card from your deck for " + _removeCardPrice + " coins.";
        public override bool IsSubscription => false;

        //Added later on when the player selects a card to remove
        public Item ItemToRemove;

        public RemoveCard(int removeCardPrice)
        {
            _removeCardPrice = removeCardPrice;
        }

        public override void ApplyReward()
        {
            //  DeckServices.RemoveCardRequest();
        }
    }
}
