using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.Effects
{

    [CreateAssetMenu(fileName = "BoostPriceEffect", menuName = "Quackery/Effects/Price/ValuePrice")]
    public class ValuePriceEffect : EffectData, IPriceModifierEffect, ICategoryEffect, IStatusEffect
    {
        [SerializeField] private EnumItemCategory _category = EnumItemCategory.Any;
        [SerializeField] private float _value;
        [SerializeField] private Status _status;
        public EnumItemCategory Category => _category;


        public float Value => _value;

        public Status Status => _status;

        public int PriceModifier(Effect effect, Card card)
        {
            if (card.Category == Category || Category == EnumItemCategory.Any)
            {
                return (int)effect.Value;
            }
            return 0;
        }

    }
}
