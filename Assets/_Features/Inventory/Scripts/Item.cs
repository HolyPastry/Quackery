using System;
using System.Collections.Generic;
using Quackery.Decks;
using Quackery.Effects;

namespace Quackery.Inventories
{
    [Serializable]
    public class Item
    {
        [NonSerialized]
        public ItemData Data;
        public string Key;

        public int NumberOfDraws = 0;

        public Item(ItemData data)
        {
            Data = data;
            Key = Data.name;

        }

        public string Name => Data.MasterText;
        public EnumItemCategory Category => OverrideCategory == EnumItemCategory.Unset ?
                                            Data.Category : OverrideCategory;

        public EnumItemCategory OverrideCategory = EnumItemCategory.Unset;
        public EnumLifetime Lifetime;

        public int BasePrice => Data.BasePrice;

        internal List<CardReward> CalculateCardRewards(Card card, List<Item> subItems, List<CardPile> otherPiles)
        {
            return Data.CalculateCardRewards(card, subItems, otherPiles);
        }


    }
}