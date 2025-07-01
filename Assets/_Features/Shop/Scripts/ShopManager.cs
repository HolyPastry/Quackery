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
            return _shopManagerData.GenerateRewards(NumRewards);
        }
    }
}
