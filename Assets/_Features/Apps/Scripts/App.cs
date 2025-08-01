using System;
using System.Collections;
using KBCore.Refs;
using UnityEngine;

namespace Quackery
{
    public class App : ValidatedMonoBehaviour
    {
        [SerializeField, Self] protected AnimatedRect _animatedRect;
        [SerializeField] protected Canvas _canvas;


        public bool IsOpened { get; private set; }

        public event Action OnStartOpening = delegate { };
        public event Action OnOpened = delegate { };
        public event Action OnStartClosing = delegate { };
        public event Action OnClosed = delegate { };

        public void Open()
        {
            transform.SetAsLastSibling();
            _canvas.gameObject.SetActive(true);
            IsOpened = true;
            OnStartOpening?.Invoke();
            _animatedRect.SlideIn(Direction.Right)
                        .DoComplete(() => OnOpened?.Invoke());
        }

        public void Close()
        {
            IsOpened = false;
            OnStartClosing.Invoke();
            StartCoroutine(HideRoutine());
        }

        private IEnumerator HideRoutine()
        {
            yield return new WaitForSeconds(1f);
            OnClosed?.Invoke();
            _canvas.gameObject.SetActive(false);
        }


    }
}
