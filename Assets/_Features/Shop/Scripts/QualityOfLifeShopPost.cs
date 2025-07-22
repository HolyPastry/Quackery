using UnityEngine;

namespace Quackery.Shops
{
    internal class QualityOfLifeShopPost : ShopPost
    {
        public override void SetupPost(ShopReward reward)
        {
            base.SetupPost(reward);
            QualityOfLifeReward qolReward = reward as QualityOfLifeReward;

            if (qolReward == null)
            {
                Debug.LogError("QoLShopPost requires a QualityOfLifeReward.");
                return;
            }


        }
    }
}
