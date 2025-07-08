using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "NumCardInCardRequirement", menuName = "Quackery/Effects/Requirements/NumCardInCardRequirement", order = 0)]
    public class NumCardInCartRequirement : EffectData, IEffectRequirement
    {


        public bool IsFulfilled(Effect effect, Card card)
        {
            return CartServices.GetNumCardInCart(EnumItemCategory.Unset) >= effect.Value;
        }
    }
}
