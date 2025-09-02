using System.Collections;
using System.Collections.Generic;
using Holypastry.Bakery.Flow;
using UnityEngine;

namespace Quackery.Effects
{
    public class SceneSetupEffects : SceneSetupScript, IEffectCarrier
    {
        [SerializeField] private List<EffectData> _initialEffects;
        public List<EffectData> EffectDataList => _initialEffects;

        public bool ActivatedCondition(Effect effect) => true;

        public override IEnumerator Routine()
        {
            yield return EffectServices.Add(this);
        }
    }
}
