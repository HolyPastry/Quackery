using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holypastry.Bakery.Flow;
using Quackery.Inventories;
namespace Quackery.Decks
{
    public class SceneSetupDeck : SceneSetupScript
    {
        [SerializeField] private List<ItemData> cards = new();
        [SerializeField] private List<ItemData> _cardInHands = new();

        protected override IEnumerator Routine()
        {
            yield return InventoryServices.WaitUntilReady();
            yield return DeckServices.WaitUntilReady();
            DeckServices.AddToDeck(cards);
            yield return null;
            DeckServices.DrawSpecificCards(_cardInHands);
            yield return null;
            EndScript();
        }
    }
}
