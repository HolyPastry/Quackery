using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using KBCore.Refs;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery
{

    public class StackInfoUI : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private Image _background;
        [SerializeField, Child] private TextMeshProUGUI _countText;
        private bool _synergyOn;
        private TweenerCore<Color, Color, ColorOptions> _colorTween;

        public void SetCount(int herbCount)
        {
            gameObject.SetActive(herbCount > 0);
            _countText.text = herbCount.ToString();
        }
        public void ClearCount()
        {
            gameObject.SetActive(false);
            _countText.text = "0";
        }
        public void SetSynergy(bool isOn)
        {
            if (isOn && !_synergyOn)
            {
                _synergyOn = true;
                transform.localScale = Vector3.one * 1.2f;
                _colorTween = _background.DOFade(0.5f, 0.5f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);

            }
            if (!isOn)
            {
                _synergyOn = false;
                _colorTween?.Kill();
                _colorTween = null;
            }

        }

    }
}
