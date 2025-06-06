using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Quackery
{
    public class TypingSign : MonoBehaviour
    {
        [SerializeField] private List<Image> _typingDots;
        [SerializeField] private float _dotAnimationDuration = 0.5f;
        [SerializeField] private float _dotAnimationDelay = 0.5f;
        void OnEnable()
        {
            StartCoroutine(AnimateTypingSign());
        }

        private IEnumerator AnimateTypingSign()
        {
            while (true)
            {
                for (int i = 0; i < _typingDots.Count; i++)
                {
                    _typingDots[i].enabled = true;
                    yield return new WaitForSeconds(_dotAnimationDuration);

                }
                yield return new WaitForSeconds(_dotAnimationDelay);
                for (int i = 0; i < _typingDots.Count; i++)
                {
                    _typingDots[i].enabled = false;
                }
            }
        }

        void OnDisable()
        {
            StopAllCoroutines();

        }
    }
}
