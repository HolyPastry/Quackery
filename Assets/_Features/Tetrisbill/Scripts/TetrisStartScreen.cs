using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery.TetrisBill
{

    public class TetrisStartScreen : MonoBehaviour
    {
        [SerializeField] private Button _startButton;
        [SerializeField] private AnimatedRect _animatedBlocks;
        [SerializeField] private int _yOffsetMiddle = 500;
        [SerializeField] private int _yOffsetTop = -15;

        [SerializeField] private GameObject _header;

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
            StartCoroutine(StartAnimation());

        }

        private IEnumerator StartAnimation()
        {
            _header.SetActive(false);
            _startButton.gameObject.SetActive(false);
            _animatedBlocks.SlideToTop(_yOffsetTop);
            yield return _animatedBlocks.WaitForAnimation();

            Hide();
            OnStart?.Invoke();
        }



        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            _header.SetActive(true);
            gameObject.SetActive(true);
            _startButton.gameObject.SetActive(true);
            _animatedBlocks.TeleportToMiddle(yOffset: _yOffsetMiddle);
        }


    }
}

