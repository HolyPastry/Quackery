using System.Collections;
using System.Collections.Generic;
using Quackery.Decks;
using Quackery.Effects;
using UnityEngine;

namespace Quackery
{
    [CreateAssetMenu(fileName = "MultitaskingEffect", menuName = "Quackery/Effects/Status/Multitasking", order = 1)]
    public class MultitaskingEffect : EffectData
    {
        public override IEnumerator Execute(Effect effect)
        {
            yield return null;
            var chance = UnityEngine.Random.Range(0, 100);
            if (chance < effect.Value)
                CardGameApp.InterruptRoundRequest();
        }
    }
}
