
using Quackery.Notifications;

namespace Quackery
{
    public class NotificationApp : App
    {
        public override void Show()
        {
            base.Show();
            NotificationServices.GenerateDailyNotification();
        }

        public override void Hide()
        {
            base.Hide();
            NotificationServices.RemoveAllNotifications();
        }
    }
}
