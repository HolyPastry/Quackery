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
            Effects = new List<Effect>();
            foreach (var effect in Data.Effects)
            {
                Effects.Add(new Effect(effect));
            }
        }

        public List<Effect> Effects;

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
            if (Effects.Count > 0)
                text = text.Replace("#EffectValue1", Effects[0].Value.ToString());
            if (Effects.Count > 1)
                text = text.Replace("#EffectValue2", Effects[0].Value.ToString());
            if (Effects.Count > 2)
                text = text.Replace("#EffectValue3", Effects[0].Value.ToString());
            if (text.Contains("#NumberDrawRemaining"))
            {
                //TODO:: really hacky, needs to find a way to do this systematically
                text = text.Replace("#NumberDrawRemaining",
                    (Effects[0].Value - NumberOfDraws).ToString());
            }

            return text;
        }

        internal List<CardReward> CalculateCardRewards(Card card, List<Item> subItems, List<CardPile> otherPiles)
        {
            return Data.CalculateCardRewards(card, subItems, otherPiles);
        }


    }
}