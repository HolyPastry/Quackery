using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "RecountCart", menuName = "Quackery/Effects/RecountCart", order = 0)]
    public class RecountCartEffect : EffectData
    {
        public override void Execute(Effect effect, CardPile pile)
        {
            DeckServices.RecountCart();
        }
    }
}
