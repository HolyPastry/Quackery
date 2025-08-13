using System.Collections;
using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{

    [CreateAssetMenu(fileName = "ModifyCartAmountEffect", menuName = "Quackery/Effects/Cart/Modify Amount", order = 0)]
    public class ModifyCartAmount : EffectData
    {
        public override IEnumerator Execute(Effect effect)
        {
            CartServices.AddToCartValue((int)effect.Value);
            yield return DefaultWaitTime;

        }
    }
}
