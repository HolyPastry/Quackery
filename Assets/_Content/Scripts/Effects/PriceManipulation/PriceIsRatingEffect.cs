using Quackery.Decks;
using Quackery.Ratings;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "PriceIsRatingEffect", menuName = "Quackery/Effects/PriceIsRating", order = 1)]
    public class PriceIsRatingEffect : CategoryEffectData
    {
        public override int PriceModifier(Effect effect, Card card)
        {
            return RatingServices.GetRating();
        }
    }
}
