
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace Quackery.Ratings
{
    public class RatingUI : MonoBehaviour
    {
        private List<Toggle> _starToggles = new();

        private bool _initialized;

        void Awake()
        {
            GetComponentsInChildren(true, _starToggles);
        }

        void OnEnable()
        {
            RatingEvents.OnRatingChanged += UpdateUI;
            if (_initialized) UpdateUI();
        }

        void OnDisable()
        {
            RatingEvents.OnRatingChanged -= UpdateUI;
        }

        IEnumerator Start()
        {
            yield return FlowServices.WaitUntilEndOfSetup();
            yield return RatingServices.WaitUntilReady();

            UpdateUI();
            _initialized = true;
        }

        // Update is called once per frame
        void UpdateUI()
        {
            var rating = RatingServices.GetRating();
            for (int i = 0; i < _starToggles.Count; i++)
                _starToggles[i].isOn = i < rating;
        }
    }
}
