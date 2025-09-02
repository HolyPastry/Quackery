using Quackery.Decks;
using Quackery.Ratings;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "PriceIsRatingEffect", menuName = "Quackery/Effects/Price/PriceIsRating", order = 1)]
    public class PriceIsRatingEffect : EffectData, IPriceModifierEffect
    {
        public float Value => 0;

        public int PriceModifier(Effect effect, Card card)
        {
            return RatingServices.GetRating();
        }


    }
}
