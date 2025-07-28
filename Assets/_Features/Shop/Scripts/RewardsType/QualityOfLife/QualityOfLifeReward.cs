using Quackery.Artifacts;
using Quackery.QualityOfLife;
using UnityEngine;

namespace Quackery.Shops
{

    public class ArtifactReward : ShopReward
    {

        public ArtifactData ArtifactData { get; private set; }

        public override int Price => ArtifactData.Price;
        public override bool IsSubscription => true;
        public override string Title => ArtifactData.MasterText;
        public override string Description => ArtifactData.Description;

        public ArtifactReward(ArtifactData artifactData)
        {
            ArtifactData = artifactData;
        }

        // public override void ApplyReward()
        // {
        //     ArtifactServices.Add(ArtifactData);
        // }
    }
}
