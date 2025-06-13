using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using Quackery.Decks;
using UnityEngine;

namespace Quackery
{
    public class PowerIcon : MonoBehaviour
    {
        [SerializeField] private Power _power;
        [SerializeField] private GameObject _activatedIcon;
        [SerializeField] private GameObject _deactivatedIcon;
        public Power Power => _power;

        public void Awake()
        {
            Reset();
        }

        public void Show(List<Power> powers)
        {
            gameObject.SetActive(powers.Contains(_power));
        }

        public void SetActive(Power power, bool isActive)
        {
            if (_power != power) return;
            _activatedIcon.SetActive(isActive);
            _deactivatedIcon.SetActive(!isActive);
        }

        internal void Reset()
        {
            gameObject.SetActive(false);
            if (_deactivatedIcon != null) _deactivatedIcon.SetActive(true);
            if (_activatedIcon != null) _activatedIcon.SetActive(false);
        }
    }
}
