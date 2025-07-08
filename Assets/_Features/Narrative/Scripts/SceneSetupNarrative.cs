
using System.Collections;
using System.Collections.Generic;
using Holypastry.Bakery.Flow;
using Quackery.Notifications;
using UnityEngine;

namespace Quackery.Narrative
{
    public class SceneSetupNarrative : SceneSetupScript
    {
        [SerializeField] private List<NarrativeData> _narrativeInProgress;
        public override IEnumerator Routine()
        {
            yield return FlowServices.WaitUntilReady();
            yield return NotificationServices.WaitUntilReady();

            foreach (var narrative in _narrativeInProgress)
            {
                NarrativeServices.AddNarrative(narrative);
            }

        }
    }
}
