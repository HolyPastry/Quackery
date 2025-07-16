using System.Collections;

using UnityEngine;
using UnityEngine.Assertions;

namespace Quackery.Notifications
{
    public class NotificationExtension : MonoBehaviour
    {
        [SerializeField] protected NotificationData _data;
        public virtual void GenerateDailyNotification()
        {
            Assert.IsNotNull(_data, "Notification Info is null");
            NotificationInfo info = new(_data);
            NotificationServices.ShowNotification(info);
        }

        public virtual bool MatchType(NotificationInfo info)
        {
            return info.Data == _data;
        }
        public virtual IEnumerator ExpandedPanelRoutine(
                        NotificationInfo info,
                        Transform expandedPanelParent)
        {
            var panel = Instantiate(info.Data.ExpandedPanelPrefab, expandedPanelParent);

            yield return panel.Init(info);
            panel.gameObject.SetActive(true);
            yield return panel.WaitUntilClosed();
        }
    }
}
