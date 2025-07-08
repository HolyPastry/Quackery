using Quackery.Narrative;

namespace Quackery.Notifications
{
    public class NarrativeNotificationExtension : NotificationExtension
    {
        public override void GenerateDailyNotification()
        {
            var narratives = NarrativeServices.GetInProgressNarratives();
            foreach (var narrative in narratives)
            {
                NarrativeNotificationInfo info = new(_data, narrative);
                NotificationServices.ShowNotification(info);
            }
        }
    }
}
