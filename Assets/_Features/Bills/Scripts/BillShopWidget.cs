
using KBCore.Refs;
using Quackery.Bills;
using TMPro;
using UnityEngine;

namespace Quackery
{
    public class BillShopWidget : ValidatedMonoBehaviour
    {
        [SerializeField, Child] private TextMeshProUGUI _billAmountText;


        public void Show(BillData billData)
        {
            _billAmountText.text = $"{billData.MasterText} (<sprite name=Coin>{billData.Price})";
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);

        }
    }
}
