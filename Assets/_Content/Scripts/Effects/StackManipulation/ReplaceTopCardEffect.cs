using System.Collections;
using System.Collections.Generic;
using Quackery.Decks;
using Quackery.Effects;
using UnityEngine;

namespace Quackery
{
    [CreateAssetMenu(fileName = "REplace Top Card", menuName = "Quackery/Effects/Stack/ReplaceTopCard", order = 0)]
    public class ReplaceTopCardEffect : MergeWithPileEffect
    {
        public override IEnumerator Execute(Effect effect)
        {
            Card card = effect.LinkedObject as Card;
            if (card == null) yield break;
            CartServices.ReplaceTopCard(card);
            yield return DefaultWaitTime;
        }
    }
}
