using System.Collections;
using System.Collections.Generic;
using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{

    [CreateAssetMenu(fileName = "SpendingLimitEffect", menuName = "Quackery/Effects/Status/SpendingLimitEffect", order = 1)]
    public class SpendingLimitEffect : EffectData
    {
        public override IEnumerator Execute(Effect effect)
        {
            if (CartServices.GetValue() >= effect.Value) yield break;

            CardGameApp.InterruptRoundRequest();
        }
    }
}
