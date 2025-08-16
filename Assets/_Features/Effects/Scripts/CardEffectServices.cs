using System;
using System.Collections.Generic;
using System.Linq;

using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;


namespace Quackery.Effects
{
    public static class CardEffectServices
    {

        public static bool IsPlayable(Card card)
        {
            return EffectServices.Get(e => e.Data is IEffectRequirement)
                     .All(requirement => (requirement.Data as IEffectRequirement)
                            .IsFulfilled(requirement, card));
        }

        public static int Price(Card card)
        {
            if (card == null) return 0;

            var effects = EffectServices.Get(e => e is IPriceModifierEffect);

            float priceModifier = effects.Sum(e => (e.Data as IPriceModifierEffect).PriceModifier(e, card));
            float ratioModifier = effects.Sum(e => (e.Data as IPriceModifierEffect).PriceMultiplier(e, card));

            float basePrice = card.Item.BasePrice;

            float categoryInCartModifier = EffectServices.GetModifier(typeof(CategoryInCartPriceEffect));
            if (categoryInCartModifier > 0)
            {
                int numCards = CartServices.GetNumCardInCart(card.Item.Category);
                basePrice = numCards * categoryInCartModifier;
            }


            int finalPrice = Mathf.RoundToInt((basePrice + priceModifier) * ratioModifier);

            return finalPrice;

        }

        public static (int multiplier, int bonus) GetSynergyBonuses(Card card, List<Item> list)
        {
            bool synergyPredicate(Effect effect) =>
                effect.Data is StackMultiplierEffect synergyEffect &&
                (synergyEffect.Category == card.Item.Category ||
                    synergyEffect.Category == EnumItemCategory.Any);

            var synergyEffects = EffectServices.Get(e => synergyPredicate(e)).ToList();

            if (synergyEffects.Count == 0) return (list.Count + 1, 0);

            int multiplier = (int)synergyEffects
                .Where(effect => effect.Data is StackMultiplierEffect synergyEffect &&
                        synergyEffect.Operation == EnumOperation.Multiply)
                .Sum(effect => effect.Value);

            int bonus = (int)synergyEffects
                .Where(effect => effect.Data is StackMultiplierEffect synergyEffect &&
                        synergyEffect.Operation == EnumOperation.Add)
                .Sum(effect => effect.Value);

            return (multiplier * (list.Count + 1), bonus);

        }

    }
}
