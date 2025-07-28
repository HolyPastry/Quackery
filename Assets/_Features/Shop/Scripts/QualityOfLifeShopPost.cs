using UnityEngine;

namespace Quackery.Shops
{
    internal class QualityOfLifeShopPost : ShopPost
    {
        public override void SetupPost(ShopReward reward)
        {
            base.SetupPost(reward);
            ArtifactReward qolReward = reward as ArtifactReward;

            if (qolReward == null)
            {
                Debug.LogError("QoLShopPost requires a QualityOfLifeReward.");
                return;
            }


        }
    }
}
