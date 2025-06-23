using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "SetConfidenceEffect", menuName = "Quackery/Effects/Set Confidence", order = 0)]
    public class SetConfidenceEffect : EffectData
    {

        public override void Execute(Effect effect, CardPile drawPile)
        {
            EffectServices.SetConfidence(effect.Value);
        }
    }
}
