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

        public int RatedPrice => Price * (Rating == 0 ? 1 : Rating);

        internal List<CardReward> CalculateCardRewards(List<Item> subItems, List<CardPile> otherPiles)
        {
            return Data.CalculateCardRewards(this, subItems, otherPiles);
        }
    }
}