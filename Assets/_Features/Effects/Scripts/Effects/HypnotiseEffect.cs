

using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(
        fileName = "HypnotiseEffect",
        menuName = "Quackery/Effects/Hypnotise",
        order = 1)]
    public class HypnotiseEffect : EffectData
    {
        public override void Execute(Effect effect, CardPile pile)
        {
            ClientServices.ForgetAilment();
            Debug.Log("Hypnotise effect executed.");

        }
    }
}
