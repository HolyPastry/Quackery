

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
        public override void Execute(Effect effect, CardPile pile)
        {
            EffectServices.RemoveAllEffects();


        }
    }
}
