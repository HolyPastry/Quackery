using System;
using System.Collections.Generic;
using Quackery.Decks;
using Quackery.Inventories;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


namespace Quackery.Shops
{
    public class CardPost : ShopPost
    {
        [Serializable]
        public struct CategoryBanner
        {
            public EnumItemCategory Category;
            public Sprite Banner;
        }

        [SerializeField] private List<CategoryBanner> _categoryBanners;
        [SerializeField] Image _banner;
        [SerializeField] TextMeshProUGUI _description;
        [SerializeField] TextMeshProUGUI _price;
        [SerializeField] Card _card;
        public override void SetupPost(ShopReward reward)
        {
            base.SetupPost(reward);
            var cardReward = reward as AddCardReward;
            Assert.IsNotNull(cardReward, "CardPost can only handle CardReward types.");

            SetCard(cardReward.ItemData);
        }

        public void SetCard(ItemData itemData)
        {
            var item = new Item(itemData);
            _banner.sprite = _categoryBanners.Find(x => x.Category == item.Category).Banner;
            _card.Item = item;
            _description.text = item.Data.Description;
            _price.text = item.BasePrice.ToString("0");
        }
    }
}
