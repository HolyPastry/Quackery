using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


namespace Quackery.Shops
{
    public class CardPost : ShopPost
    {

        [SerializeField] Image _frame;
        [SerializeField] Image _categoryIcon;
        [SerializeField] Image _icon;
        [SerializeField] TextMeshProUGUI _nameTextGUI;
        [SerializeField] TextMeshProUGUI _descriptionTextGUI;
        [SerializeField] TextMeshProUGUI _priceTextGUI;

        public override void SetupPost(ShopReward reward)
        {
            base.SetupPost(reward);
            var cardReward = reward as NewCardReward;
            Assert.IsNotNull(cardReward, "CardPost can only handle CardReward types.");

            var itemData = cardReward.ItemData;

            _frame.color = Colors.Get(itemData.Category.ToString());
            _categoryIcon.sprite = Sprites.GetCategory(itemData.Category);
            _icon.sprite = itemData.Icon;
            _nameTextGUI.text = itemData.MasterText;
            _descriptionTextGUI.text = itemData.ShortDescription;
            _priceTextGUI.text = itemData.BasePrice.ToString();

        }
    }
}
