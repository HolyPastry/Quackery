using System.Collections;
using Holypastry.Bakery.Flow;
using UnityEngine;

namespace Quackery.Followers
{
    public class SceneSetup_Followers : SceneSetupScript
    {
        [SerializeField] private int _numberOfFollowers = 0;
        public override IEnumerator Routine()
        {
            yield return FlowServices.WaitUntilReady();
            yield return FollowerServices.WaitUntilReady();

            FollowerServices.SetNumberOfFollowers(_numberOfFollowers);
        }
    }
}
