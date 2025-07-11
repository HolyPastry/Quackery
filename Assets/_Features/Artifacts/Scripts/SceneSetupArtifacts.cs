using System.Collections;
using System.Collections.Generic;
using Holypastry.Bakery.Flow;
using UnityEngine;

namespace Quackery.Artifacts
{
    public class SceneSetupArtifacts : SceneSetupScript
    {
        [SerializeField] private List<ArtifactData> _artifacts;
        public override IEnumerator Routine()
        {
            yield return FlowServices.WaitUntilReady();
            yield return ArtifactServices.WaitUntilReady();

            foreach (var artifact in _artifacts)
            {
                if (artifact == null)
                {
                    Debug.LogWarning("Artifact is null, skipping.", this);
                    continue;
                }
                ArtifactServices.Add(artifact);
            }
        }
    }
}
