
using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "EndTransactionEffect", menuName = "Quackery/Effects/Price/End Transaction", order = 2)]
    public class EndTransactionEffect : EffectData
    {
        public override void Execute(Effect effect)
        {
            CardGameController.EndTransactionRequest();
        }

    }
}
