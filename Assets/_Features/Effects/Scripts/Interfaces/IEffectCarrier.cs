using System.Collections.Generic;


namespace Quackery.Effects
{
    public interface IEffectCarrier
    {
        public List<EffectData> EffectDataList { get; }

        public bool ActivatedCondition(Effect effect);
    }
}
