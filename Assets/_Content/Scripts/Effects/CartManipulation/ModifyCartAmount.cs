using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "ModifyCartAmountEffect", menuName = "Quackery/Effects/Modify Cart Amount", order = 0)]
    public class ModifyCartAmount : EffectData
    {
        public override void Execute(Effect effect)
        {
            CartServices.AddToCartValue(effect.Value);
        }
    }
}
