using System;
using System.Collections.Generic;
using UnityEngine;



namespace Quackery.Effects
{
    public class PlayerEffects : MonoBehaviour, IEffectCarrier
    {
        public List<EffectData> EffectDataList => throw new NotImplementedException();
        public bool ActivatedCondition(Effect effect) => true;

    }
}
