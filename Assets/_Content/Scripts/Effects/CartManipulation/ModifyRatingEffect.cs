using System.Collections;
using Quackery.Decks;
using Quackery.Ratings;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "ModifyRating", menuName = "Quackery/Effects/Cart/Modify Rating", order = 0)]
    public class ModifyRatingEffect : EffectData
    {
        public override IEnumerator Execute(Effect effect)
        {
            RatingServices.Modify((int)effect.Value);
            yield return Tempo.WaitForABeat;
        }

    }
}
