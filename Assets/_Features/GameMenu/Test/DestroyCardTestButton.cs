using KBCore.Refs;
using Quackery.Inventories;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery.GameMenu.Test
{
    public class DestroyCardTestButton : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private Button _button;
        [SerializeField] private ItemData _itemData;

        // Start is called before the first frame update
        void Start()
        {
            _button.onClick.AddListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            GameMenuController.RemoveFromDeckRequest?.Invoke(_itemData, true);
        }
    }
}
