using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        // public override string Description => "<b>Bundle:</b> Allows you to bundle up to #Value cards together.";

        public override IEnumerator ExecutePile(Effect effect, CardPile owningPile)
        {

            if (effect.LinkedObject == null || (effect.LinkedObject as Card) == null)
            {
                Debug.LogWarning("Bundle Effect not setup properly: " + effect.Data.name);
                yield break;
            }
            var cards = DeckServices
                    .GetMatchingCards((card) => card.Category == (effect.LinkedObject as Card).Category
                        , EnumCardPile.Hand);


            yield return DeckServices.MoveToPile(cards, owningPile);

        }
    }
}
