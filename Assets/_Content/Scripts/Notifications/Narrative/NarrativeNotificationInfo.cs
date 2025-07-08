using Quackery.Narrative;

namespace Quackery.Notifications
{
    public class NarrativeNotificationInfo : NotificationInfo
    {
        public NarrativeData Narrative { get; private set; }

        public NarrativeNotificationInfo(NotificationData data, NarrativeData narrative) : base(data)
        {
            Narrative = narrative;
        }

    }
}
