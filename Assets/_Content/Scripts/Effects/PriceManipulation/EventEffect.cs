using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "Event Effect", menuName = "Quackery/Effects/Price/Event Effect")]
    public class EventEffect : EffectData, IPriceModifierEffect, ICategoryEffect, IStatusEffect
    {
        [SerializeField] private int _priceModifier;
        [SerializeField] private EnumItemCategory _category;
        [SerializeField] private Status _status;
        [SerializeField] private EnumEffectTrigger _trigger;

        public float Value => _priceModifier;
        public EnumItemCategory Category => _category;
        public Status Status => _status;
        public EnumEffectTrigger Trigger => _trigger;

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
