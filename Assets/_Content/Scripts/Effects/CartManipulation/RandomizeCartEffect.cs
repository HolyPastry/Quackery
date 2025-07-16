using System.Collections;
using System.Collections.Generic;
using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "RandomizeCart", menuName = "Quackery/Effects/Cart/Randomize Cart")]
    public class RandomizeCartEffect : EffectData
    {
        public override IEnumerator Execute(Effect effect)
        {
            CartServices.RandomizeCart();
            yield return DefaultWaitTime;
        }
    }
}
