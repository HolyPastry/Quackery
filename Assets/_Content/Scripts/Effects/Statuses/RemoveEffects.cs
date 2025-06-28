

using System.Collections.Generic;
using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(
        fileName = "RemoveEffects",
        menuName = "Quackery/Effects/RemoveEffects",
        order = 2)]
    public class RemoveEffects : EffectData
    {
        [Tooltip("Effect to remove.")]
        [SerializeField] private List<EffectData> _effectsToRemove;

        public override string GetDescription() => $"Remove effects";

        public override void Execute(Effect effect)
        {
            foreach (var effectToRemove in _effectsToRemove)
            {
                EffectServices.Cancel(effectToRemove);
            }
        }
    }
}
