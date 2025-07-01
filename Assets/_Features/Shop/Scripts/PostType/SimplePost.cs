using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


namespace Quackery.Shops
{
    public class SimplePost : ShopPost
    {
        [SerializeField] Image _banner;
        [SerializeField] TextMeshProUGUI _description;
        [SerializeField] TextMeshProUGUI _price;


        public Sprite RemoveCardBanner;
        public Sprite UpgradeCardBanner;

        public override void SetupPost(ShopReward reward)
        {
            base.SetupPost(reward);
            if (reward is RemoveCardReward)
            {
                _banner.sprite = RemoveCardBanner;
                _description.text = "Remove a card from your deck.";
                _price.text = reward.Price.ToString("0");
            }
            else if (reward is UpgradeCard)
            {
                _banner.sprite = UpgradeCardBanner;
                _description.text = "Upgrade a card in your deck.";
                _price.text = reward.Price.ToString("0");
            }
            else
            {
                Debug.LogError("SimplePost can only handle RemoveCardReward and UpgradeCardReward types.");
            }

        }
    }
}
