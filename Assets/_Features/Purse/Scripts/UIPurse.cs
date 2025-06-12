using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quackery
{
    public class UIPurse : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI _purseText;
        void OnEnable()
        {
            PurseEvents.OnPurseUpdated += OnPurseUpdated;
        }

        void OnDisable()
        {
            PurseEvents.OnPurseUpdated -= OnPurseUpdated;
        }


        IEnumerator Start()
        {
            yield return PurseServices.WaitUntilReady();
            _purseText.text = PurseServices.GetString();
        }
        private void OnPurseUpdated(float _)
        {
            _purseText.text = PurseServices.GetString();
        }
    }
}
