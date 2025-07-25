using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.Effects
{


    [CreateAssetMenu(fileName = "BoostPriceEffect", menuName = "Quackery/Effects/Price/ValuePrice")]
    public class ValuePriceEffect : EffectData, IPriceModifierEffect, ICategoryEffect
    {
        [SerializeField] private EnumItemCategory _category = EnumItemCategory.Any;
        public EnumItemCategory Category => _category;

        public int PriceModifier(Effect effect, Card card)
        {
            if (card.Category == Category || Category == EnumItemCategory.Any)
            {
                return effect.Value;
            }
            return 0;
        }

    }
}
