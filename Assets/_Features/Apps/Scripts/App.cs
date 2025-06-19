using System.Collections;
using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;

namespace Quackery
{
    public class App : ValidatedMonoBehaviour
    {
        [SerializeField, Self] protected AnimatedRect _animatedRect;
        [SerializeField] protected Canvas _canvas;

        public bool IsOn { get; private set; }

        public virtual void Show()
        {
            _canvas.gameObject.SetActive(true);
            IsOn = true;
            _animatedRect.SlideIn(Direction.Right);
        }

        public virtual void Hide()
        {
            IsOn = false;
            _animatedRect.SlideOut(Direction.Left)
                         .DoComplete(() => _canvas.gameObject.SetActive(false));
        }
    }
}
