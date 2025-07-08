using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.Effects
{


    [CreateAssetMenu(fileName = "BoostPriceEffect", menuName = "Quackery/Effects/Price/ValuePrice")]
    public class ValuePriceEffect : EffectData, IPriceModifierEffect, ICategoryEffect
    {
        [SerializeField] private EnumItemCategory _category = EnumItemCategory.Unset;
        public EnumItemCategory Category => _category;

        public int PriceModifier(Effect effect, Card card)
        {
            if (card.Category == Category || Category == EnumItemCategory.Unset)
            {
                return effect.Value;
            }
            return 0;
        }

        public float PriceMultiplier(Effect effect, Card card) => 0;

    }
}
