using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quackery.Effects
{


    [CreateAssetMenu(fileName = "HandSizeEffect", menuName = "Quackery/Effects/Status/Hand Size", order = 1)]
    public class HandSizeEffect : EffectData, IStatusEffect
    {
        [SerializeField] private int _modifier;
        [SerializeField] private Status _status;
        public float Value => _modifier;
        public Status Status => _status;
    }
}