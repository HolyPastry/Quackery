using System.Collections.Generic;
using Holypastry.Bakery;
using TMPro;
using UnityEngine;

namespace Quackery.Shops
{

    public class ShopManager : MonoBehaviour
    {
        [SerializeField] private ShopManagerData _shopManagerData;

        void OnEnable()
        {
            ShopServices.GetRewards = GetRewards;
            ShopServices.ApplyReward = ApplyReward;
        }

        void OnDisable()
        {
            ShopServices.GetRewards = (amount) => new();
            ShopServices.ApplyReward = (data) => { };
        }

        private void ApplyReward(ShopReward reward)
        {
            reward.ApplyReward();
        }

        private List<ShopReward> GetRewards(int NumRewards)
        {
            List<ShopRewardType> rewardTypes = _shopManagerData.GenerateRewards(NumRewards);

            List<ShopReward> rewards = new();
            foreach (var type in rewardTypes)
            {
                ShopReward reward = type switch
                {
                    ShopRewardType.AddCard => new AddCardReward(),
                    ShopRewardType.RemoveCard => new RemoveCard(_shopManagerData.RemoveCardPrice),
                    ShopRewardType.UpgradeCard => new UpgradeCard(_shopManagerData.UpgradeCardPrice),
                    _ => null
                };
                if (reward != null)
                    rewards.Add(reward);
            }
            return rewards;
            //var reward = new ShopReward();
        }
    }
}
