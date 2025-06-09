using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery.Inventories
{
    public class ItemUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _itemNameText;
        [SerializeField] private TextMeshProUGUI _itemCountText;
        [SerializeField] private Image _itemIcon;
        private Item _item;
        public Item Item => _item;
        public void SetItem(Item item)
        {
            _item = item;
            _itemNameText.text = item.Name;

            _itemCountText.text = item.Quantity.ToString();
            _itemIcon.sprite = item.Data.Icon;
        }

    }
}
