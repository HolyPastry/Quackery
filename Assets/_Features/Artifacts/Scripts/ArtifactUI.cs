using System;
using System.Collections;
using System.Collections.Generic;
using Holypastry.Bakery.Flow;
using UnityEngine;
using UnityEngine.UI;




namespace Quackery.Artifacts
{
    public class SceneSetupArtifacts : SceneSetupScript
    {
        [SerializeField] private List<ArtifactData> _artifacts;
        public override IEnumerator Routine()
        {
            yield return FlowServices.WaitUntilEndOfSetup();
            yield return ArtifactServices.WaitUntilReady();

            foreach (var artifact in _artifacts)
            {
                ArtifactServices.Add(artifact);
            }
        }
    }
    public class ArtifactUI : MonoBehaviour
    {

        [SerializeField] private Image Icon;
        private ArtifactData _artifactData;

        public ArtifactData Artifact
        {
            get => _artifactData;
            set
            {
                _artifactData = value;
                Icon.sprite = value.Icon;
            }
        }

    }
}
