
using System.Collections;
using Quackery.Effects;
using Quackery.Followers;
using UnityEngine;

namespace Quackery
{
    [CreateAssetMenu(fileName = "Earn1foreachXfollowers", menuName = "Quackery/Effects/Followers/Earn 1 for each X followers", order = 1)]
    public class FollowerMonetizationEffect : EffectData
    {

        public override IEnumerator Execute(Effect effect)
        {
            int numFollowers = FollowerServices.GetNumberOfFollowers();
            int cash = numFollowers / effect.Value;
            if (cash > 0)
                PurseServices.Modify(cash);
            yield return null;

        }
    }
}
