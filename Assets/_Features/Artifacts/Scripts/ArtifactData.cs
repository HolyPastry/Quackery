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

        public int Level;

        public List<ArtifactData> Requirements = new();
        public List<Explanation> Explanations = new();
        public bool IsUpgrade => UpgradeFor != null;

        public EnumRarity Rarity;

        public List<Effect> Effects = new();
    }
}
