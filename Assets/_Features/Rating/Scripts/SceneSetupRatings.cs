using System.Collections;
using Holypastry.Bakery.Flow;
using UnityEngine;


namespace Quackery.Ratings
{
    public class SceneSetupRatings : SceneSetupScript
    {
        [SerializeField] private int _startRating = 0;
        protected override IEnumerator Routine()
        {
            yield return FlowServices.WaitUntilReady();
            yield return RatingServices.WaitUntilReady();

            RatingServices.Modify(_startRating);
            EndScript();
        }
    }
}
