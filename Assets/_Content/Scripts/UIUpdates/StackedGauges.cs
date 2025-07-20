using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery
{


    [ExecuteAlways]
    public class StackedGauges : MonoBehaviour
    {
        [SerializeField] protected List<Image> _gauges = new();
        [SerializeField] protected List<TextMeshProUGUI> _gaugeValues = new();

        void Update()
        {
            if (_gauges.Count < 2)
                return;
            if (_gauges.Count != _gaugeValues.Count)
            {
                Debug.LogWarning("Gauges and Gauge Values lists must have the same number of elements.");
                return;
            }
            float width = 0;
            for (int i = 0; i < _gauges.Count; i++)
            {
                var gauge = _gauges[i];
                var gaugeValue = _gaugeValues[i];
                if (gauge == null)
                    continue;
                var rectTransform = gauge.transform as RectTransform;
                rectTransform.anchoredPosition = new Vector2(width, rectTransform.anchoredPosition.y);


                var gaugeValueRectTransform = gaugeValue.transform as RectTransform;
                gaugeValueRectTransform.anchoredPosition
                     = new Vector2(width +
                         rectTransform.rect.width * gauge.fillAmount / 2
                         , gaugeValueRectTransform.anchoredPosition.y);

                width += rectTransform.rect.width * gauge.fillAmount;
            }
        }
    }
}
