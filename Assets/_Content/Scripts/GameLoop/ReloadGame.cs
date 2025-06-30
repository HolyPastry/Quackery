using System;
using System.Collections;
using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Quackery
{
    public class ReloadGame : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private Button _button;

        void OnEnable()
        {
            _button.onClick.AddListener(OnReload);
        }

        void OnDisable()
        {
            _button.onClick.RemoveListener(OnReload);
        }

        private void OnReload()
        {
            DG.Tweening.DOTween.KillAll();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }


    }
}
