using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holypastry.Bakery.Flow;
using Quackery.Inventories;
namespace Quackery.Decks
{
    public class SceneSetupDeck : SceneSetupScript
    {
        [SerializeField] private List<ItemData> _cardInHands = new();
        [SerializeField] private List<ItemData> _itemDataList = new();

        [SerializeField] private bool _addAllItems = true;

        public override IEnumerator Routine()
        {
            yield return InventoryServices.WaitUntilReady();
            //   yield return DeckServices.WaitUntilReady();
            if (_addAllItems)
            {
                InventoryServices.AddNewItems(null);
            }
            else
            {
                InventoryServices.AddNewItems(_itemDataList);
            }

            yield return null;

        }

        public void DrawCards()
        {
            DeckServices.DrawSpecificCards(_cardInHands);
        }


    }
}
