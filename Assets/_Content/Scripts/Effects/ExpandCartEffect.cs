using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "EffectData", menuName = "Quackery/Effects/ExpandCart", order = 0)]
    public class ExpandCartEffect : EffectData
    {
        public override void Execute(Effect effect, CardPile pile)
        {
            DeckServices.ExpandCart(effect.Value);
        }
    }
}
