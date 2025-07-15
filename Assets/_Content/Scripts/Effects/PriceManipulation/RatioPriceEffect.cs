using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "RatioPriceEffect", menuName = "Quackery/Effects/Price/Ratio Price")]
    public class RatioPriceEffect : EffectData, IPriceModifierEffect, ICategoryEffect
    {
        public float Ratio = 1.0f;

        [SerializeField] private EnumItemCategory _category = EnumItemCategory.Any;
        public EnumItemCategory Category => _category;

        public float PriceMultiplier(Effect effect, Card card)
        {
            if (card.Category == Category || Category == EnumItemCategory.Any)
            {
                return Ratio;
            }
            return 0;
        }
    }
}
