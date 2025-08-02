using System.Collections.Generic;
using Quackery.Artifacts;
using Quackery.Inventories;
using Quackery.Progression;
using UnityEngine;

namespace Quackery.Shops
{

    public class ShopManager : MonoBehaviour
    {
        [SerializeField] private ShopManagerData _shopManagerData;

        void OnEnable()
        {
            ShopServices.GetRewards = GetRewards;
            // ShopServices.ApplyReward = ApplyReward;
        }

        void OnDisable()
        {
            ShopServices.GetRewards = (amount) => new();
            //  ShopServices.ApplyReward = (data) => { };
        }

        // private void ApplyReward(ShopReward reward)
        // {
        //     reward.ApplyReward();
        // }

        private List<ShopReward> GetRewards(int NumRewards)
        {
            var shopRewardTypes = _shopManagerData.GenerateRewards(NumRewards);
            var rewards = new List<ShopReward>();

            int currentLevel = ProgressionServices.GetLevel();

            foreach (var (rewardType, amount) in shopRewardTypes)
            {
                switch (rewardType)
                {
                    case ShopRewardType.NewCard:

                        for (int i = 0; i < amount; i++)
                        {
                            _shopManagerData.RollRarity(currentLevel, out EnumRarity rarity);
                            ItemData itemData = InventoryServices.GetRandomMatchingItem(
                                        itemData => itemData.Category != EnumItemCategory.Curse &&
                                        itemData.Category != EnumItemCategory.TempCurse &&
                                        itemData.Rarity == rarity);
                            if (itemData == null)
                            {
                                Debug.LogWarning("Shop - Rolling for a new card. no matching card found for: " + rarity);
                                continue;
                            }
                            rewards.Add(new NewCardReward(itemData));
                        }

                        break;

                    case ShopRewardType.RemoveCard:
                        List<RemovalCompany> removalCompanies = _shopManagerData.GetRandomRemovalCompanies(amount);

                        for (int i = 0; i < Mathf.Min(amount, removalCompanies.Count); i++)
                            rewards.Add(new RemoveCardReward(removalCompanies[i]));

                        break;

                    case ShopRewardType.Artifact:
                        List<ArtifactData> artifactData = ArtifactServices.GetRandomSuitable(currentLevel, amount);
                        for (int i = 0; i < Mathf.Min(amount, artifactData.Count); i++)
                            rewards.Add(new ArtifactReward(artifactData[i]));

                        break;

                    default:
                        Debug.LogWarning($"Unknown reward type: {rewardType}");
                        break;
                }

            }

            return rewards;
        }
    }
}
