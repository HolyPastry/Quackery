using System;
using System.Collections.Generic;
using UnityEngine;

namespace Quackery.Shops
{
    [CreateAssetMenu(fileName = "ShopManagerData", menuName = "Quackery/Shop/ShopManagerData", order = 1)]
    public class ShopManagerData : ScriptableObject
    {
        public int RemoveCardPrice = 10;
        public int UpgradeCardPrice = 20;



        [Serializable]
        public struct RewardDistribution
        {
            public ShopRewardType Type;
            public int Weight; // Higher weight means more likely to be chosen, -1 means it's always available;
        }

        public List<RewardDistribution> RewardDistributions = new();

        public List<ShopRewardType> GenerateRewards(int amount)
        {
            List<ShopRewardType> rewards = new();

            List<RewardDistribution> weightedDistrib = new();

            foreach (var distribution in RewardDistributions)
                if (distribution.Weight < 0) // Always available rewards
                    rewards.Add(distribution.Type);
                else
                    weightedDistrib.Add(distribution);

            for (int i = 0; i < amount; i++)
            {
                int totalWeight = 0;
                foreach (var distribution in weightedDistrib)
                {
                    if (distribution.Weight >= 0)
                        totalWeight += distribution.Weight;
                }

                int randomValue = UnityEngine.Random.Range(0, totalWeight);
                int cumulativeWeight = 0;
                int index = 0;
                while (index < weightedDistrib.Count && cumulativeWeight <= randomValue)
                {
                    cumulativeWeight += weightedDistrib[index].Weight;
                    index++;
                }
                if (index == 0)
                    break; // No valid rewards
                index--; // Get the last valid index
                rewards.Add(weightedDistrib[index].Type);
                weightedDistrib.RemoveAt(index);
            }

            return rewards;
        }

    }
}
