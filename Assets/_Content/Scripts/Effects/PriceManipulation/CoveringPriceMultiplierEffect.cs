using System;
using System.Collections;
using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;


namespace Quackery.Effects
{


    [CreateAssetMenu(fileName = "New Effect", menuName = "Quackery/Effects/Price/Covering Price Multiplier Effect", order = 1)]
    public class CoveringPriceMultiplierEffect : EffectData, ICategoryEffect
    {
        [SerializeField] private EnumItemCategory _category;
        public EnumItemCategory Category => _category;

        public override IEnumerator ExecutePile(Effect effect, CardPile pile)
        {
            if (pile.Count <= 1) yield break;
            // if (pile._cards[1].Category != _category) yield break;
            int price = CardEffectServices.Price(pile.TopCard);
            CartServices.AddToCartValue(price);
        }
    }
}
