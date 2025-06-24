
using System;
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

        public string Description;
        public int SubscriptionCost;

        public EnumItemCategory Category;

        public List<Effect> Effects = new();

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


            if (subItems.Count > 0)
            {
                rewards.Add(new()
                {
                    Type = EnumCardReward.StackReward,
                    Value = subItems.Count
                    //TODO: Count the stacks properly using Effects
                    //Value = EffectServices.GetValue(topCard, subItems)
                });
            }
            int numSameCategory = 0;
            foreach (var pile in otherPiles)
            {
                if (pile.Category == topCard.Item.Category)
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

        internal string GetDescription()
        {
            return Sprites.ReplaceCategories(Description);
        }
    }
}