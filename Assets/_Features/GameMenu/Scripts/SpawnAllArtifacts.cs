using Quackery.Artifacts;
using UnityEngine;

namespace Quackery.GameMenu
{

    public class SpawnAllArtifacts : MonoBehaviour
    {
        [SerializeField] private ArtifactCollectionItem _artifactPrefab;

        void OnEnable()
        {
            SpawnAll();
        }

        void OnDisable()
        {
            DestroyAll();
        }

        private void SpawnAll()
        {
            var artifacts = ArtifactServices.GetAllArtifacts();
            foreach (var artifact in artifacts)
            {
                var item = Instantiate(_artifactPrefab, transform);
                item.Initialize(artifact);
            }
        }
        private void DestroyAll()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
