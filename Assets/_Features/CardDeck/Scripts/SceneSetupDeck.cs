using System.Collections;
using UnityEngine;
using Holypastry.Bakery.Flow;
using System.Collections.Generic;
using Quackery.Inventories;
namespace Quackery.Decks
{
    public class SceneSetupDeck : SceneSetupScript
    {



        [Tooltip("Import all cards if _deck is left empty.")]
        [SerializeField] private DeckData _deck;



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

        }




    }
}
