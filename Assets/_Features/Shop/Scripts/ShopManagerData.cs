using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Quackery.Shops
{
    [Serializable]
    public struct CardRarityChances
    {
        public int Common;
        public int Uncommon;
        public int Rare;

        public readonly EnumRarity Roll()
        {
            int total = Common + Uncommon + Rare;
            int randomValue = UnityEngine.Random.Range(0, total + 1);

            if (randomValue < Common)
                return EnumRarity.Common;
            else if (randomValue < Common + Uncommon)
                return EnumRarity.Uncommon;
            else
                return EnumRarity.Rare;
        }
    }

    [Serializable]
    public class RemovalCompany
    {
        public string Name;
        public Sprite Logo;

        [TextArea(3, 10)]
        public string Description;
        public int Price;
    }

    [CreateAssetMenu(fileName = "ShopManagerData", menuName = "Quackery/Shop/ShopManagerData", order = 1)]
    public class ShopManagerData : SerializedScriptableObject
    {
        public int RemoveCardPrice = 10;
        public int UpgradeCardPrice = 20;

        public List<CardRarityChances> CardRarityChances = new();


        [SerializeField] private List<RemovalCompany> _removalCompanies;

        [Serializable]
        public struct RewardDistribution
        {
            public ShopRewardType Type;
            public int Weight; // Higher weight means more likely to be chosen, -1 means it's always available;
        }

        public List<RewardDistribution> RewardDistributions = new();

        public List<(ShopRewardType, int)> GenerateRewards(int amount)
        {
            List<ShopRewardType> rewards = new();

            List<RewardDistribution> weightedDistrib = new();

            foreach (var distribution in RewardDistributions)
                if (distribution.Weight < 0)
                {
                    rewards.Add(distribution.Type);
                }
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

            List<(ShopRewardType, int)> rewardsAmount = new();
            foreach (var rewardType in Enum.GetValues(typeof(ShopRewardType)))
            {
                int count = rewards.Count(r => r == (ShopRewardType)rewardType);
                if (count > 0)
                    rewardsAmount.Add(((ShopRewardType)rewardType, count));
            }

            return rewardsAmount;
        }

        internal List<RemovalCompany> GetRandomRemovalCompanies(int amount)
        {
            List<RemovalCompany> randomCompanies = new();

            if (amount <= 0 || _removalCompanies == null || _removalCompanies.Count == 0)
                return randomCompanies;

            _removalCompanies.Shuffle();

            for (int i = 0; i < Mathf.Min(amount, _removalCompanies.Count); i++)
                randomCompanies.Add(_removalCompanies[i]);
            return randomCompanies;
        }

        internal void RollRarity(int currentLevel, out EnumRarity rarity)
        {
            if (currentLevel >= CardRarityChances.Count)
            {
                rarity = EnumRarity.Common;
                Debug.LogWarning("Current level exceeds the number of defined rarity chances. Defaulting to Common.");
                return;
            }

            rarity = CardRarityChances[currentLevel].Roll();
        }
    }
}
