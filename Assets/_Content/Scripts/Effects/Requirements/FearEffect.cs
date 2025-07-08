
using Quackery.Decks;
using Quackery.Effects;
using Quackery.Ratings;
using UnityEngine;

namespace Quackery
{
    [CreateAssetMenu(fileName = "FearEffect", menuName = "Quackery/Effects/Statuses/Fear Effect", order = 0)]
    public class FearEffect : EffectData, IEffectRequirement
    {
        public bool IsFulfilled(Effect effect, Card card)
        {
            var rating = RatingServices.GetRating();

            // Fear effects are always fulfilled, as they are meant to be applied without conditions.
            return card.Price <= rating;
        }
    }
}
