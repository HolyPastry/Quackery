using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "PriceModifierBasedOnPriceEffect", menuName = "Quackery/Effects/Price/Price Modifier Based On Price", order = 0)]
    public class PriceModifierBasedOnPriceEffect : EffectData, IPriceModifierEffect
    {
        [SerializeField] private int _priceToMatch = 1;


        public int PriceModifier(Effect effect, Card card)
        {
            if (card.Item.BasePrice == _priceToMatch)
            {
                return effect.Value;
            }
            return 0;
        }

    }
}
