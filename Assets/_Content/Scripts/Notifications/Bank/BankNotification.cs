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
            _message.text = Sprites.Replace($"You have {PurseServices.GetString()}!");
            _billDueToday.text = $"{BillServices.GetNumBillDueToday()} bill(s) are due this week";
        }

    }
}
