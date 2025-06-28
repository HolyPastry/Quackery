using System.Collections;
using System.Collections.Generic;
using Quackery.Decks;
using Quackery.Effects;
using UnityEngine;

namespace Quackery
{
    [CreateAssetMenu(fileName = "REplace Top Card", menuName = "Quackery/Effects/ReplaceTopCard", order = 0)]
    public class ReplaceTopCardEffect : MergeWithPreviousPileEffect
    {
        public override void Execute(Effect effect)
        {
            //DeckServices.ReplaceTopCard();
        }
    }
}
