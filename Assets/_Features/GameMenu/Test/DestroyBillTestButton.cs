using KBCore.Refs;
using Quackery.Bills;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery.GameMenu.Test
{
    public class DestroyBillTestButton : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private Button _button;
        [SerializeField] private BillData _billData;

        // Start is called before the first frame update
        void Start()
        {
            _button.onClick.AddListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            GameMenuController.RemoveFromBillsRequest?.Invoke(_billData);
        }
    }
}
