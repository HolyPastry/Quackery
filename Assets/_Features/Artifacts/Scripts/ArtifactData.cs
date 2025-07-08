using System.Collections.Generic;
using Holypastry.Bakery;
using UnityEngine;




namespace Quackery.Artifacts
{


    [CreateAssetMenu(menuName = "Quackery/ArtifactData")]
    public class ArtifactData : ContentTag
    {
        public Sprite Icon;
        [TextArea(3, 10)]
        public string Description;

        public ArtifactData UpgradeFor;
        public bool IsUpgrade => UpgradeFor != null;

        public List<Effect> Effects;
    }
}
