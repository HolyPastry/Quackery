using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.Effects
{

    [CreateAssetMenu(fileName = "BoostPriceEffect", menuName = "Quackery/Effects/BoostPrice")]
    public class BoostPriceEffect : EffectData
    {
        public EnumItemCategory Category;

        public override void Execute(Effect effect, CardPile pile)
        {
            //noop
        }
        public override int PriceModifier(Effect effect, Card card)
        {
            if (card.Category == Category)
            {
                return effect.Value;
            }
            return 0;

        }
    }
}
