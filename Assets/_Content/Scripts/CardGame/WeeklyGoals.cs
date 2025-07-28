
using System.Collections.Generic;
using System.Linq;
using Quackery.Clients;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery
{
    public class WeeklyGoals : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _segments = new();
        [SerializeField] private Image _gauge;
        [SerializeField] private TextMeshProUGUI _valueText;
        [SerializeField] private RectTransform _today;

        void OnEnable()
        {
            PurseEvents.OnPurseUpdated += UpdateUI;
            UpdateUI(0);
        }

        void OnDisable()
        {
            PurseEvents.OnPurseUpdated -= UpdateUI;
        }

        private void UpdateUI(float obj)
        {
            var purseAmount = PurseServices.GetAmount();
            //  var billDue = BillServices.GetAmountDueToday();

            var clientList = ClientServices.GetClientList();
            var numClientServed = clientList.Where(c => c.Served).Count();

            var numSegments = clientList.Count - 1;

            // _gauge.fillAmount = (float)purseAmount / billDue;
            // _valueText.text = Sprites.Replace($"#Coin {purseAmount}/{billDue}");
            var width = _gauge.rectTransform.rect.width;
            var segmentWidth = width / numSegments;

            _today.anchoredPosition = new Vector2(segmentWidth / 2 + numClientServed * segmentWidth, 0);

            for (int i = 0; i < _segments.Count; i++)
            {
                _segments[i].SetActive(i <= numSegments);
                if (i >= numSegments) continue;

                var rectTransform = _segments[i].transform as RectTransform;
                rectTransform.anchoredPosition = new Vector2(i * segmentWidth, 0);
            }

        }
    }
}
