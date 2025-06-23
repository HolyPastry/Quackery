using System;
using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{

    [CreateAssetMenu(fileName = "MergeCart", menuName = "Quackery/Effects/MergeCart", order = 0)]
    public class MergeCartEffect : EffectData
    {
        public override void Execute(Effect effect, CardPile pile)
        {
            DeckServices.MergeCart(effect.Value);
        }
    }
}
