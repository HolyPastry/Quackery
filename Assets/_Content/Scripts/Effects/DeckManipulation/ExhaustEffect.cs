
using Quackery.Decks;
using Quackery.Effects;
using UnityEngine;

namespace Quackery
{
    [CreateAssetMenu(fileName = "ExhaustEffect", menuName = "Quackery/Effects/Exhaust", order = 0)]
    public class ExhaustEffect : EffectData
    {
        public override void Execute(Effect effect)
        {
            if (effect.LinkedCard != null)
            {
                //DeckServices.MoveCardTo(effect.LinkedCard);
            }
            else
            {
                Debug.LogWarning("Card to be destroyed is null.");
            }
        }
    }
}
