using System;
using UnityEngine;
using UnityEngine.UI;




namespace Quackery.Artifacts
{
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
