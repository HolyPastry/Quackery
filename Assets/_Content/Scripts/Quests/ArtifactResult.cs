
using Holypastry.Bakery.Quests;
using Quackery.Artifacts;
using UnityEngine;

namespace Quackery
{
    [CreateAssetMenu(fileName = "ArtifactResult", menuName = "Quackery/Quests/Artifact Result")]
    public class ArtifactResult : Result
    {
        [SerializeField] private ArtifactData _artifact;
        public override void Execute()
        {
            ArtifactServices.Add(_artifact);
        }
    }
}
