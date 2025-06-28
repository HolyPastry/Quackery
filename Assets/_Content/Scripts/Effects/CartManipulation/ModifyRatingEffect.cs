using Quackery.Decks;
using Quackery.Ratings;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "ModifyRating", menuName = "Quackery/Effects/Modify Rating", order = 0)]
    public class ModifyRatingEffect : EffectData
    {
        public override void Execute(Effect effect)
        {
            RatingServices.Modify(effect.Value);
        }

    }
}
