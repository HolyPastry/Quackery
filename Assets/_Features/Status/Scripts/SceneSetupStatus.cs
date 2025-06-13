using System.Collections;
using System.Collections.Generic;
using Holypastry.Bakery.Flow;
using UnityEngine;

namespace Quackery
{
    public class SceneSetupStatus : SceneSetupScript
    {
        [SerializeField] private List<StatusData> _initialStatuses;
        protected override IEnumerator Routine()
        {
            foreach (var statusData in _initialStatuses)
            {
                // Add initial statuses to the StatusManager
                StatusServices.AddStatus(statusData, Vector2.zero);
            }
            yield return null;
            EndScript();
        }
    }
}
