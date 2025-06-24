using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "ConfidenceEffect", menuName = "Quackery/Effects/ConfidenceEffect", order = 1)]
    public class ConfidenceEffect : EffectData
    {
        public override void Cancel(Effect effect)
        {
            //noop
        }

        public override void Execute(Effect effect, CardPile drawPile)
        {
            if (effect.Value <= 0) CardGameController.InterruptRoundRequest();
        }
    }
}
