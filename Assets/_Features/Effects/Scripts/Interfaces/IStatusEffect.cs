using UnityEngine;

namespace Quackery.Effects
{
    public interface IStatusEffect : IValueEffect
    {
        public Status Status { get; }
       
        public Sprite Icon => Status != null ? Status.Icon : null;

         
    }
}
