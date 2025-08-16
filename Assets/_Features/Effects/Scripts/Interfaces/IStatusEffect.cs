using UnityEngine;

namespace Quackery.Effects
{
    public interface IStatusEffect
    {
        public Status Status { get; }
        public EnumEffectTrigger Trigger { get; }

        public Sprite Icon => Status != null ? Status.Icon : null;
    }
}
