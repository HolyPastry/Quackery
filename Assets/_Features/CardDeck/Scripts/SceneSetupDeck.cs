using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holypastry.Bakery.Flow;
using Quackery.Inventories;
using Quackery.Effects;
namespace Quackery.Decks
{
    public class SceneSetupDeck : SceneSetupScript
    {
        [SerializeField] private List<ItemData> _cardInHands = new();

        [Tooltip("Import all cards if _deck is left empty.")]
        [SerializeField] private DeckData _deck;

        [SerializeField] private List<ItemData> _placeOnTopOfDeck = new();

        public override IEnumerator Routine()
        {
            yield return InventoryServices.WaitUntilReady();
            if (_deck == null)
            {
                InventoryServices.AddNewItems(null);
            }
            else
            {
                _deck.AddToInventory();
            }
            foreach (var itemData in _placeOnTopOfDeck)
            {
                DeckServices.AddNew(itemData,
                        EnumCardPile.Draw,
                        EnumPlacement.OnTop,
                        EnumLifetime.Temporary, true);
            }
        }

        public void DrawCards()
        {
            DeckServices.DrawSpecificCards(_cardInHands);
        }


    }
}
