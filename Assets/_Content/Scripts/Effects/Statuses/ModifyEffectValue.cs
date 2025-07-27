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
            if (effect.ContainsTag(EnumEffectTag.Duration))
                return;
            EffectServices.ModifyValue(_effectToModify, -effect.Value);
        }

        public override IEnumerator Execute(Effect effect)
        {
            int value = effect.Value;
            if (effect.ContainsTag(EnumEffectTag.Duration))
                value = 1;
            int countered = 0;
            if (value > 0)
                foreach (var counterEffect in _counterEffects)
                    countered += EffectServices.CounterEffect(counterEffect, value - countered);

            EffectServices.ModifyValue(_effectToModify, value - countered);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
