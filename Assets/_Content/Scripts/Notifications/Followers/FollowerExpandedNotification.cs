
using System.Collections;

using Quackery.Followers;
using Quackery.Notifications;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery
{
    public class FollowerExpandedNotification : NotificationExpandedPanel
    {
        [SerializeField] private Image _currentLevelIcon;
        [SerializeField] private Image _nextLevelIcon;
        [SerializeField] private TextMeshProUGUI _currentLevelTitle;
        [SerializeField] private TextMeshProUGUI _nextLevelTitle;
        [SerializeField] private TextMeshProUGUI _currentLevelNumber;
        [SerializeField] private TextMeshProUGUI _nextLevelNumber;
        [SerializeField] private TextMeshProUGUI _currentNumber;
        [SerializeField] private RectTransform _currentLevelPanel;

        public override IEnumerator Init(NotificationInfo info)
        {
            yield return base.Init(info);

            var currentLevel = FollowerServices.GetCurrentLevel();
            var nextLevel = FollowerServices.GetNextLevel();
            var numFollowers = FollowerServices.GetNumberOfFollowers();
            _currentLevelIcon.sprite = currentLevel.Icon;
            _nextLevelIcon.sprite = nextLevel.Icon;
            _currentLevelTitle.text = currentLevel.Name;
            _nextLevelTitle.text = nextLevel.Name;
            _currentLevelNumber.text = currentLevel.FollowerRequirement.ToString();
            _nextLevelNumber.text = nextLevel.FollowerRequirement.ToString();
            _currentNumber.text = numFollowers.ToString();

            float nextLevelY = _nextLevelIcon.rectTransform.anchoredPosition.y - 120;
            float currentLevelY = _currentLevelIcon.rectTransform.anchoredPosition.y + 120;
            float levelY = Mathf.Lerp(
                        currentLevelY,
                        nextLevelY,
                         (numFollowers - currentLevel.FollowerRequirement) /
                         (nextLevel.FollowerRequirement - currentLevel.FollowerRequirement));
            _currentLevelPanel.anchoredPosition = new Vector2(_currentLevelPanel.anchoredPosition.x, levelY);

        }

    }
}
