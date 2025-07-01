
using Quackery.Inventories;
using Quackery.Shops;
using UnityEngine;

namespace Quackery
{

    public class RemoveCardReward : ShopReward
    {

        public RemovalCompany SelectedCompany { get; private set; }

        public override int Price => SelectedCompany.Price;

        public override string Description => SelectedCompany.Description;
        public override bool IsSubscription => true;
        public override string Title => SelectedCompany.Name;
        public Sprite Logo => SelectedCompany.Logo;

        public RemoveCardReward(RemovalCompany company)
        {
            SelectedCompany = company;
        }

        //Added later on when the player selects a card to remove
        public Item ItemToRemove;

        public override void ApplyReward()
        {
            // if (ItemToRemove != null)
            //     DeckServices.DestroyCard(ItemToRemove);
        }
    }
}
