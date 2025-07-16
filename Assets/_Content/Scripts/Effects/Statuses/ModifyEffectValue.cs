using System.Collections;
using System.Collections.Generic;
using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{

    [CreateAssetMenu(fileName = "ModifyEffectValue", menuName = "Quackery/Effects/Status/Modify Effect Value", order = 1)]
    public class ModifyEffectValue : EffectData
    {
        [SerializeField] private EffectData _effectToModify;
        [SerializeField] private List<EffectData> _counterEffects = new();

        public override void Cancel(Effect effect)
        {
            EffectServices.ModifyValue(_effectToModify, -effect.Value);
        }

        public override IEnumerator Execute(Effect effect)
        {
            int countered = 0;
            if (effect.Value > 0)
                foreach (var counterEffect in _counterEffects)
                    countered += EffectServices.CounterEffect(counterEffect, effect.Value - countered);

            EffectServices.ModifyValue(_effectToModify, effect.Value - countered);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
