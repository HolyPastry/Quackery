
using System.Collections.Generic;
using System.Linq;
using Holypastry.Bakery;
using Quackery.Decks;
using Quackery.Effects;
using UnityEngine;

namespace Quackery.Inventories
{

    [CreateAssetMenu(
        fileName = "ItemData",
        menuName = "Quackery/ItemData",
        order = 1)]
    public class ItemData : ContentTag
    {
        public Sprite Icon;

        public int StartPrice = 1;
        public int StartRating = 0;

        public EnumItemCategory Category;

        public List<EffectData> Effects = new();

        public ValueEvaluator ValueEvaluator;

        internal List<CardReward> CalculateCardRewards(Card topCard, List<Item> subItems, List<CardPile> otherPiles)
        {

            List<CardReward> rewards = new()
            {
                new()
                {
                    Type = EnumCardReward.BaseReward,
                    Value = topCard.Price
                }
            };

            if (topCard.Rating > 0)
                rewards.Add(new()
                {
                    Type = EnumCardReward.RatingReward,
                    Value = topCard.Rating
                });
            if (subItems.Count > 0)
            {
                rewards.Add(new()
                {
                    Type = EnumCardReward.StackReward,
                    Value = subItems.Sum(subItem => subItem.Price)
                });
            }
            int numSameCategory = 0;
            foreach (var pile in otherPiles)
            {
                if (pile.Category == topCard.Item.Data.Category)
                {
                    numSameCategory++;
                }
            }
            if (numSameCategory > 0)
            {
                rewards.Add(new()
                {
                    Type = EnumCardReward.NeighborReward,
                    Value = numSameCategory
                });
            }


            if (ValueEvaluator != null)
            {
                rewards.AddRange(ValueEvaluator.Evaluate(topCard, subItems, otherPiles));
            }
            return rewards;
        }
    }
}