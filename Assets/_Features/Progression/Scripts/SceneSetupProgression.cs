using System.Collections;
using Holypastry.Bakery.Flow;
using UnityEngine;

namespace Quackery.Progression
{
    public class SceneSetupProgression : SceneSetupScript
    {
        [SerializeField] private int _startLevel = 0;
        public override IEnumerator Routine()
        {
            yield return FlowServices.WaitUntilReady();
            yield return ProgressionServices.WaitUntilReady();
            ProgressionServices.SetLevel(_startLevel);
        }
    }
}
