

using System;
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

        public int BasePrice = 1;

        [TextArea(2, 10)]
        public string ShortDescription;

        [TextArea(3, 10)]
        public string LongDescription;

        [TextArea(3, 10)]
        public string ShopDescription;

        public List<Explanation> Explanations = new();

        public int SubscriptionCost;

        public EnumItemCategory Category;

        public List<Effect> Effects = new();

        public ValueEvaluator ValueEvaluator;

        internal List<CardReward> CalculateCardRewards(Card topCard, List<Item> subItems, List<CardPile> otherPiles)
        {

            List<CardReward> rewards = new();
            // {
            //     new()
            //     {
            //         Type = EnumCardReward.BaseReward,
            //         Value = topCard.Price
            //     }
            // };
            (int multiplier, int bonus) = EffectServices.GetSynergyBonuses(topCard, subItems);

            if (subItems.Count > 0 && subItems.TrueForAll(i => i.Category == topCard.Category))
            {
                rewards.Add(new()
                {
                    Type = EnumCardReward.Synergy,
                    Value = (topCard.Price + bonus) * multiplier
                });
            }

            // int value = EffectServices.GetStackPrice(topCard, subItems);
            // if (value > 0)
            // {
            //     rewards.Add(new()
            //     {
            //         Type = EnumCardReward.StackReward,
            //         Value = value
            //     });
            // }
            // int numSameCategory = 0;
            // foreach (var pile in otherPiles)
            // {
            //     if (pile.Category == topCard.Item.Category)
            //     {
            //         numSameCategory++;
            //     }
            // }
            // if (numSameCategory > 0)
            // {
            //     rewards.Add(new()
            //     {
            //         Type = EnumCardReward.NeighborReward,
            //         Value = numSameCategory
            //     });
            // }


            if (ValueEvaluator != null)
            {
                rewards.AddRange(ValueEvaluator.Evaluate(topCard, subItems, otherPiles));
            }
            return rewards;
        }

        internal void CheckValidity()
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogWarning("ItemData must have a name.");
            }
            if (Icon == null)
            {
                Debug.LogWarning($"ItemData {name} must have an icon.");
            }
            Effects.ForEach(effect =>
            {
                if (effect == null)
                {
                    Debug.LogWarning($"ItemData {name} has a null effect.");
                }
                else
                {
                    effect.CheckValidity();
                }
            });
        }
    }
}