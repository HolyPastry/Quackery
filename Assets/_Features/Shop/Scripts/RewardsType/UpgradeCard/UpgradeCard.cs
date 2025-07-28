using Quackery.Inventories;
using Quackery.Shops;
using UnityEngine;

namespace Quackery
{
    public class UpgradeCard : ShopReward
    {
        private int _price;



        public override int Price => _price;
        public override string Description => "Upgrade a card to enhance its abilities or stats.";
        public override bool IsSubscription => false;

        public override string Title => throw new System.NotImplementedException();

        //Added later on when the player selects a card to remove
        public Item ItemToUpgrade;

        public UpgradeCard(int price)
        {
            _price = price;
            // This constructor can be used to initialize any properties if needed.
            // Currently, it does not require any specific initialization.
        }
        // public override void ApplyReward()
        // {
        //     // This is a placeholder for the upgrade logic.
        //     // The actual implementation would depend on how cards are upgraded in the game.
        //     // For example, it could involve increasing the card's stats or abilities.
        //     Debug.Log("Card upgrade logic not implemented yet.");
        // }
    }
}
