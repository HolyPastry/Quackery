using System;
using System.Collections.Generic;
using UnityEngine;



namespace Quackery.Effects
{
    public class PlayerEffects : MonoBehaviour, IEffectCarrier
    {
        public List<EffectData> EffectDataList => new();
        public bool ActivatedCondition(Effect effect) => true;

    }
}
