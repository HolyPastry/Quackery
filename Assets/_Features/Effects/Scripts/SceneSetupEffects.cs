using System.Collections;
using System.Collections.Generic;
using Holypastry.Bakery.Flow;
using UnityEngine;

namespace Quackery.Effects
{
    public class SceneSetupEffects : SceneSetupScript
    {
        [SerializeField] private List<Effect> _initialEffects;
        [SerializeField] private Transform _effectOrigin;
        public override IEnumerator Routine()
        {
            foreach (var effect in _initialEffects)
            {
                EffectServices.Add(effect);
            }
            yield return null;
        }
    }
}
