using System.Collections;
using Quackery.Decks;
using UnityEngine;


namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "CartValueMultiplier", menuName = "Quackery/Effects/Price/Cart Value Multiplier", order = 1)]
    public class CartValueMultiplier : EffectData
    {

        public override IEnumerator Execute(Effect effect)
        {
            if (effect.LinkedCard == null) yield break;

            int value = CartServices.GetValue();
            CartServices.AddToCartValue(Mathf.FloorToInt(value * ((float)effect.Value / 100)));
        }
    }
}
