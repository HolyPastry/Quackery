

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

        public override IEnumerator Execute(Effect effect)
        {
            yield return null;
            EffectServices.Remove(e => _effectsToRemove.Contains(e.Data));
        }
    }
}
