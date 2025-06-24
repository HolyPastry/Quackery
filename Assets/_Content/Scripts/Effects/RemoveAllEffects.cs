

using System.Collections.Generic;
using Quackery.Clients;
using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(
        fileName = "RemoveAllEffects",
        menuName = "Quackery/Effects/RemoveAllEffects",
        order = 1)]
    public class RemoveAllEffects : EffectData
    {
        [Tooltip("List of effects that should not be removed.")]
        [SerializeField] private List<EffectData> _whiteList;
        public override void Execute(Effect effect, CardPile pile)
        {
            EffectServices.CancelAllEffects(_whiteList);
        }
    }
}
