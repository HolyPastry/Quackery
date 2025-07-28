using TMPro;
using UnityEngine;

namespace Quackery.Notifications
{
    public class BankNotification : Notification
    {
        [SerializeField] private TextMeshProUGUI _message;
        [SerializeField] private TextMeshProUGUI _billDueToday;
        protected override void SetInfo(NotificationInfo _)
        {
            _message.text = PurseServices.GetString();
            // int billDueToday = BillServices.GetAmountDueToday();
            // if (billDueToday > 0)
            // {
            //     _billDueToday.text = $"{billDueToday.ToString("0")}\n due today!";
            //     _billDueToday.gameObject.SetActive(true);
            // }
            // else
            // {
            //     _billDueToday.gameObject.SetActive(false);
            // }
        }

    }
}
