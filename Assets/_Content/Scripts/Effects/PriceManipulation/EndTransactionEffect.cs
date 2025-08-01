
using System.Collections;
using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "EndTransactionEffect", menuName = "Quackery/Effects/Price/End Transaction", order = 2)]
    public class EndTransactionEffect : EffectData
    {
        public override IEnumerator Execute(Effect effect)
        {
            CardGameApp.EndTransactionRequest();
            yield return DefaultWaitTime;
        }

    }
}
