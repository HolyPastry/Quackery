using System;
using System.Collections.Generic;
using Quackery.Inventories;
using Quackery.Shops;
using UnityEngine;

namespace Quackery
{
    [CreateAssetMenu(fileName = "RemoveCardReward", menuName = "Quackery/Shop/Remove Card Reward")]
    public class RemoveCardReward : ShopReward
    {
        [Serializable]
        public class RemovalCompany
        {
            public string Name;
            public Sprite Logo;

            [TextArea(3, 10)]
            public string Description;
            public int Price;
        }

        [SerializeField] private List<RemovalCompany> _removalCompanies;

        private RemovalCompany _selectedCompany;

        private RemovalCompany SelectedCompany
        {
            get
            {
                _selectedCompany ??= _removalCompanies[UnityEngine.Random.Range(0, _removalCompanies.Count)];
                return _selectedCompany;
            }
        }
        public override int Price => SelectedCompany.Price;

        public override string Description => SelectedCompany.Description;
        public override bool IsSubscription => true;
        public override string Title => SelectedCompany.Name;
        public Sprite Logo => SelectedCompany.Logo;

        //Added later on when the player selects a card to remove
        [HideInInspector]
        public Item ItemToRemove;

        public override void ApplyReward()
        {
            // if (ItemToRemove != null)
            //     DeckServices.DestroyCard(ItemToRemove);
        }
    }
}
