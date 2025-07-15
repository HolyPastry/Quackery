using System;
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

        public override void ExecutePile(Effect effect, CardPile pile)
        {
            if (pile.Count <= 1) return;
            if (pile.Cards[1].Category != _category) return;
            int price = EffectServices.GetCardPrice(pile.TopCard);
            CartServices.AddToCartValue(price);
        }
    }
}
