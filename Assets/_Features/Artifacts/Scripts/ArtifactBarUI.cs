using UnityEngine;




namespace Quackery.Artifacts
{
    public class ArtifactBarUI : MonoBehaviour
    {
        [SerializeField] private ArtifactUI _prefab;
        void OnEnable()
        {
            ArtifactEvents.OnArtifactAdded += OnArtifactAdded;
            ArtifactEvents.OnArtifactUpdated += OnArtifactUpdated;
        }

        void OnDisable()
        {
            ArtifactEvents.OnArtifactAdded -= OnArtifactAdded;
            ArtifactEvents.OnArtifactUpdated -= OnArtifactUpdated;
        }

        private void OnArtifactAdded(ArtifactData data)
        {
            if (data.IsUpgrade)
            {
                foreach (var child in transform)
                {
                    var artifactUI = child as ArtifactUI;
                    if (artifactUI.Artifact == data.UpgradeFor)
                    {
                        artifactUI.Artifact = data;
                        break;
                    }
                }
                return;
            }

            var ui = Instantiate(_prefab, transform);
            ui.Artifact = data;

        }

        private void OnArtifactUpdated(ArtifactData data)
        {

        }
    }
}
