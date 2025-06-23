using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{

    [CreateAssetMenu(fileName = "ChangeConfidenceEffect", menuName = "Quackery/Effects/ChangeConfidence", order = 1)]
    public class ChangeConfidenceEffect : EffectData
    {

        public override void Execute(Effect effect, CardPile drawPile)
        {
            EffectServices.ModifyConfidence(effect.Value);
        }
    }
}
