using System.Collections.Generic;
using Holypastry.Bakery;
using Quackery.Inventories;
using Quackery.QualityOfLife;
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
            var shopRewardTypes = _shopManagerData.GenerateRewards(NumRewards);
            var rewards = new List<ShopReward>();

            foreach (var (rewardType, amount) in shopRewardTypes)
            {
                switch (rewardType)
                {
                    case ShopRewardType.NewCard:
                        List<ItemData> itemDatas = InventoryServices.GetRandomItems(amount);
                        for (int i = 0; i < Mathf.Min(amount, itemDatas.Count); i++)
                            rewards.Add(new NewCardReward(itemDatas[i]));

                        break;

                    case ShopRewardType.RemoveCard:
                        List<RemovalCompany> removalCompanies = _shopManagerData.GetRandomRemovalCompanies(amount);

                        for (int i = 0; i < Mathf.Min(amount, removalCompanies.Count); i++)
                            rewards.Add(new RemoveCardReward(removalCompanies[i]));

                        break;

                    case ShopRewardType.QualityOfLife:
                        List<QualityOfLifeData> qualityOfLifeDatas = QualityOfLifeServices.GetRandomSuitable(amount);
                        for (int i = 0; i < Mathf.Min(amount, qualityOfLifeDatas.Count); i++)
                            rewards.Add(new QualityOfLifeReward(qualityOfLifeDatas[i]));

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
