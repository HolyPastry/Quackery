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
        [SerializeField] private List<ItemData> _cardsInDeck = new();

        [SerializeField] private bool _addAllCards = true;

        public override IEnumerator Routine()
        {
            yield return InventoryServices.WaitUntilReady();
            //   yield return DeckServices.WaitUntilReady();
            if (_addAllCards)
            {
                InventoryServices.AddNewItems(null);
            }
            else
            {
                InventoryServices.AddNewItems(_cardsInDeck);
            }

            yield return null;

        }

        public void DrawCards()
        {
            DeckServices.DrawSpecificCards(_cardInHands);
        }


    }
}
