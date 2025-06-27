using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "PriceModifierBasedOnPriceEffect", menuName = "Quackery/Effects/Price Modifier Based On Price", order = 0)]
    public class PriceModifierBasedOnPriceEffect : EffectData
    {
        [SerializeField] private int _priceToMatch = 1;
        public override void Execute(Effect effect, CardPile pile)
        {
            //noop
        }

        public override int PriceModifier(Effect effect, Card card)
        {
            if (card.Item.BasePrice == _priceToMatch)
            {
                return effect.Value;
            }
            return 0;
        }

        public override void Cancel(Effect effect)
        { }
    }
}
