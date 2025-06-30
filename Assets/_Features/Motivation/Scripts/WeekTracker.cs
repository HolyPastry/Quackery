
using KBCore.Refs;
using TMPro;
using UnityEngine;

namespace Quackery
{
    public class WeekTracker : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private TextMeshProUGUI _weekTextGUI;

        void OnEnable()
        {
            UpdateWeekText();
        }

        private void UpdateWeekText()
        {
            int currentWeek = CalendarServices.Today();
            _weekTextGUI.text = $"{currentWeek}";
        }
    }
}
