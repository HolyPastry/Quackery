

using System.Collections;
using System.Collections.Generic;
using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(
        fileName = "RemoveEffects",
        menuName = "Quackery/Effects/Status/RemoveEffects",
        order = 2)]
    public class RemoveEffects : EffectData
    {
        [Tooltip("Effect to remove.")]
        [SerializeField] private List<EffectData> _effectsToRemove;

        // public override string GetDescription() => $"Remove effects";

        public override IEnumerator Execute(Effect effect)
        {
            foreach (var effectToRemove in _effectsToRemove)
            {
                EffectServices.Cancel(effectToRemove);
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}
