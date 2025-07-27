using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quackery.Inventories;
namespace Quackery.Decks
{
    public class AddCardOnTopOfDeck : MonoBehaviour
    {
        [SerializeField] private List<ItemData> _cards = new();

        IEnumerator Start()
        {
            yield return FlowServices.WaitUntilEndOfSetup();
            yield return DeckServices.WaitUntilReady();
            AddOnTopOfDeck();
        }

        public void AddOnTopOfDeck()
        {
            DeckServices.ForceOnNextDraw(_cards);
        }
    }
}
