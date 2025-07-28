
using Holypastry.Bakery.Quests;
using Quackery.Artifacts;
using UnityEngine;

namespace Quackery
{
    [CreateAssetMenu(fileName = "ArtifactCondition", menuName = "Quackery/Quests/Conditions/Artifact Condition")]
    public class ArtifactCondition : Condition
    {
        [SerializeField] private ArtifactData _artifact;
        public override bool Check => ArtifactServices.Owns(_artifact);
    }
}
