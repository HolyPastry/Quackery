using System.Collections;
using System.Collections.Generic;
using Holypastry.Bakery.Flow;
using UnityEngine;

namespace Quackery.Effects
{
    public class SceneSetupEffects : SceneSetupScript
    {
        [SerializeField] private List<EffectData> _initialEffects;
        [SerializeField] private Transform _effectOrigin;
        protected override IEnumerator Routine()
        {
            foreach (var data in _initialEffects)
            {
                Effect effect = new(data, initValue: true)
                {
                    Origin = _effectOrigin.position
                };
                EffectServices.Add(effect);
            }
            yield return null;
            EndScript();
        }
    }
}
