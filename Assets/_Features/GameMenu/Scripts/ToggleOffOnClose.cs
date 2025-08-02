using System;
using System.Collections;
using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery.GameMenu
{
    public class ToggleOffOnClose : ValidatedMonoBehaviour
    {

        [SerializeField, Self] private Toggle _toggle;

        void OnEnable()
        {
            GameMenuController.OnGameMenuToggle += HandleGameMenuToggle;
        }
        void OnDisable()
        {
            GameMenuController.OnGameMenuToggle -= HandleGameMenuToggle;
        }

        private void HandleGameMenuToggle(bool isOn)
        {
            if (!isOn)
                _toggle.isOn = false;
        }
    }
}
