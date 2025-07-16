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
            CartServices.ReplaceTopCard(effect.LinkedCard);
            yield return DefaultWaitTime;
        }
    }
}
