using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "StackMultiplier", menuName = "Quackery/Effects/StackMultiplier", order = 0)]
    public class StackMultiplierEffect : EffectData
    {
        public override void Execute(Effect effect, CardPile pile)
        {
            EffectServices.IncreaseStackReward(effect.Value);
        }
    }
}
