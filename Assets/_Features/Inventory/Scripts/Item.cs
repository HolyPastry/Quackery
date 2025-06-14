using System;
using System.Collections.Generic;
using Quackery.Decks;

namespace Quackery.Inventories
{
    [Serializable]
    public class Item
    {
        [NonSerialized]
        public ItemData Data;
        public string Key;

        public Item(ItemData data)
        {
            Data = data;
            Key = Data.name;

        }

        public string Name => Data.MasterText;

        public int Price;

        public int Rating;

        internal List<CardReward> CalculateCardRewards(Card card, List<Item> subItems, List<CardPile> otherPiles)
        {
            return Data.CalculateCardRewards(card, subItems, otherPiles);
        }


    }
}