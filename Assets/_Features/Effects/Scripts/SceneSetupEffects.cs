using System.Collections;
using System.Collections.Generic;
using Holypastry.Bakery.Flow;
using UnityEngine;

namespace Quackery.Effects
{
    public class SceneSetupEffects : SceneSetupScript
    {
        [SerializeField] private List<EffectData> _initialEffects;
        protected override IEnumerator Routine()
        {
            foreach (var data in _initialEffects)
            {
                EffectServices.Add(new(data));
            }
            yield return null;
            EndScript();
        }
    }
}
