using System;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery.TetrisBill
{

    public class TetrisStartScreen : MonoBehaviour
    {
        [SerializeField] private Button _startButton;

        public event Action OnStart = delegate { };

        void OnEnable()
        {
            _startButton.onClick.AddListener(OnStartButtonClicked);

        }
        void OnDisable()
        {
            _startButton.onClick.RemoveListener(OnStartButtonClicked);
        }

        private void OnStartButtonClicked()
        {
            Hide();
            OnStart?.Invoke();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }


    }
}

