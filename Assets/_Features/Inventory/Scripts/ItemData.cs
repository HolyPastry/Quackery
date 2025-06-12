
using System.Collections.Generic;
using System.Linq;
using Holypastry.Bakery;
using Quackery.Decks;
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

        public List<Power> Powers;

        public ValueEvaluator ValueEvaluator;

        internal List<CardReward> CalculateCardRewards(Item mainItem, List<Item> subItems, List<CardPile> otherPiles)
        {

            List<CardReward> rewards = new()
            {
                new()
                {
                    Type = EnumCardReward.BaseReward,
                    Value = mainItem.Price
                }
            };

            if (mainItem.Rating > 0)
                rewards.Add(new()
                {
                    Type = EnumCardReward.RatingReward,
                    Value = mainItem.Rating
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
                if (pile.Category == mainItem.Data.Category)
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
                rewards.AddRange(ValueEvaluator.Evaluate(mainItem, subItems, otherPiles));
            }
            return rewards;
        }
    }
}