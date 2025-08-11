using System.Collections;
using System.Collections.Generic;
using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(
         fileName = "BundlePower",
         menuName = "Quackery/Effects/Stack/Bundle",
         order = 1)]
    public class BundleEffect : EffectData
    {
        public override void OnRemove(Effect effect)
        {

        }

        // public override string Description => "<b>Bundle:</b> Allows you to bundle up to #Value cards together.";

        public override IEnumerator ExecutePile(Effect effect, CardPile owningPile)
        {
            List<CardPile> piles = DeckServices.GetTablePile();

            foreach (var pile in piles)
            {
                if (pile.Category != effect.LinkedCard.Category) continue;
                DeckServices.MoveToPile(pile, owningPile);

            }
            yield return null;

        }
    }
}
