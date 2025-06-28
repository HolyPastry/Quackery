using System.Collections.Generic;
using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{


    [CreateAssetMenu(fileName = "SpendingLimitEffect", menuName = "Quackery/Effects/SpendingLimitEffect", order = 1)]
    public class SpendingLimitEffect : EffectData
    {

        public override void Execute(Effect effect)
        {
            if (CartServices.GetCartValue() >= effect.Value) return;

            CardGameController.InterruptRoundRequest();
        }
    }
}
