namespace Quackery.Notifications
{
    public class FollowerMonetizationNotificationInfo : NotificationInfo
    {
        public FollowerMonetizationNotificationInfo(NotificationData data, int cash) : base(data)
        {
            EarnedCash = cash;
        }

        public int EarnedCash { get; private set; }
    }
}
