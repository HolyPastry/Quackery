
using Quackery.Decks;
using Quackery.Effects;
using UnityEngine;

namespace Quackery
{
    [CreateAssetMenu(fileName = "ExhaustEffect", menuName = "Quackery/Effects/Exhaust", order = 0)]
    public class ExhaustEffect : EffectData
    {
        public override void Execute(Effect effect, CardPile pile)
        {
            if (effect.LinkedCard != null)
            {
                DeckServices.DestroyCard(effect.LinkedCard);
            }
            else
            {
                Debug.LogWarning("Card to be destroyed is null.");
            }
        }
    }
}
