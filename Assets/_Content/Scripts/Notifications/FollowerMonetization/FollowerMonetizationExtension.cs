namespace Quackery.Notifications
{
    public class FollowerMonetizationExtension : NotificationExtension
    {
        public override void GenerateDailyNotification()
        {
            var cashEarned = EffectServices.GetModifier(
                                typeof(FollowerMonetizationEffect));

            if (cashEarned <= 0) return;

            NotificationServices.ShowNotification(
                new FollowerMonetizationNotificationInfo(_data, cashEarned));

        }
    }
}
