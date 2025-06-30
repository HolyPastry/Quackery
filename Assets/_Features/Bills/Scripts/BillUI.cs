using System;


using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace Quackery.Bills
{
    public class BillUI : MonoBehaviour
    {
        [SerializeField] private Image _billIcon;
        [SerializeField] private TextMeshProUGUI _billTitle;
        [SerializeField] private TextMeshProUGUI _billAmount;
        // [SerializeField] private TextMeshProUGUI _billDueDate;
        [SerializeField] private Button _payButton;
        // [SerializeField] private TextMeshProUGUI _billWarningText;
        [SerializeField] private GameObject _paidIndicator;

        private Bill _bill;

        public event Action<Bill> OnPayButtonClicked = delegate { };


        void OnEnable()
        {
            _payButton.onClick.AddListener(OnPayButtonClickedHandler);
            PurseEvents.OnPurseUpdated += UpdatePayButtonState;
        }

        void OnDisable()
        {
            _payButton.onClick.RemoveListener(OnPayButtonClickedHandler);
            PurseEvents.OnPurseUpdated -= UpdatePayButtonState;
        }

        private void UpdatePayButtonState(float obj)
        {
            _payButton.interactable = PurseServices.CanAfford(_bill.TotalPrice);
        }

        private void OnPayButtonClickedHandler()
        {
            BillServices.PayBill(_bill);
            //   _billDueDate.gameObject.SetActive(false);
            //   _billWarningText.gameObject.SetActive(false);
            _payButton.gameObject.SetActive(false);
            _paidIndicator.SetActive(true);

        }

        public void Initialize(Bill bill)
        {
            _paidIndicator.SetActive(false);
            _bill = bill;
            _billIcon.sprite = bill.Data.Icon;
            _billTitle.text = bill.Title;
            _billAmount.text = bill.TotalPrice.ToString("C");
            int dueIn = BillServices.DueIn(bill);

            // if (dueIn == 0)
            // {
            //  _billWarningText.text = "Bill due Today!";
            //  _billDueDate.gameObject.SetActive(false);
            //  _billWarningText.gameObject.SetActive(true);
            // }
            // else if (dueIn < 0)
            // {
            //     _billWarningText.text = "Overdue!";
            //     _billDueDate.gameObject.SetActive(false);
            //     _billWarningText.gameObject.SetActive(true);
            // }
            // else
            // {
            //     _billDueDate.text = $"Due in {dueIn} days";
            //     _billDueDate.gameObject.SetActive(true);
            //     _billWarningText.gameObject.SetActive(false);
            // }
            _payButton.gameObject.SetActive(true);
            _payButton.interactable = PurseServices.CanAfford(bill.TotalPrice);
        }
    }
}
