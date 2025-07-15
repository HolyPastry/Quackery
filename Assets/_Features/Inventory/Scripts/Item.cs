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
        public EnumItemCategory Category => OverrideCategory == EnumItemCategory.Any ?
                                            Data.Category : OverrideCategory;

        public EnumItemCategory OverrideCategory = EnumItemCategory.Any;
        public EnumLifetime Lifetime;

        public int BasePrice => Data.BasePrice;

        public string ShortDescription => SwapValues(Data.ShortDescription);
        public string LongDescription => SwapValues(Data.LongDescription);
        public string ShopDescription => SwapValues(Data.ShopDescription);


        public string SwapValues(string text)
        {
            text = text.Replace("#DrawnTime", NumberOfDraws.ToString());
            if (Data.Effects.Count > 0)
                text = text.Replace("#EffectValue1", Data.Effects[0].Value.ToString());
            if (Data.Effects.Count > 1)
                text = text.Replace("#EffectValue2", Data.Effects[0].Value.ToString());
            if (Data.Effects.Count > 2)
                text = text.Replace("#EffectValue3", Data.Effects[0].Value.ToString());
            return text;
        }

        internal List<CardReward> CalculateCardRewards(Card card, List<Item> subItems, List<CardPile> otherPiles)
        {
            return Data.CalculateCardRewards(card, subItems, otherPiles);
        }


    }
}