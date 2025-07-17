using System;
using System.Collections;
using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Quackery
{
    public class RebootButton : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private Button _button;

        void OnEnable()
        {
            _button.onClick.AddListener(OnReboot);
        }
        void OnDisable()
        {
            _button.onClick.RemoveListener(OnReboot);
        }

        private void OnReboot()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
