using System;
using System.Collections;
using System.Collections.Generic;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.Decks
{
    [CreateAssetMenu(fileName = "DeckData", menuName = "Quackery/DeckData", order = 0)]
    public class DeckData : ScriptableObject
    {
        public List<ItemData> Cards = new();

        internal void AddToInventory()
        {
            InventoryServices.AddNewItems(Cards);
        }
    }
}
