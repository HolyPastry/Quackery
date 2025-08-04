using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace Quackery.GameMenu
{
    public class PurseUpdate : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textGUI;
        [SerializeField] private TextMeshProUGUI _bonusTextGUI;
        private int _value;

        public RectTransform RectTransform => transform as RectTransform;

        void Awake()
        {
            _bonusTextGUI.gameObject.SetActive(false);

        }

        public void UpdateUI()
        {
            _value = PurseServices.GetAmount();
            _textGUI.text = $"<sprite name=Money>{PurseServices.GetString()}";
            LayoutRebuilder.ForceRebuildLayoutImmediate(RectTransform);

        }

        public IEnumerator AddMoney(int amount)
        {
            _bonusTextGUI.rectTransform.anchoredPosition =
                    new Vector2(_bonusTextGUI.rectTransform.anchoredPosition.x, 0);
            _bonusTextGUI.gameObject.SetActive(false);
            UpdateUI();
            if (amount == 0) yield break;
            if (amount > 0)
            {
                _bonusTextGUI.text = $"+{amount}";
                _bonusTextGUI.color = Color.green;
            }
            else
            {
                _bonusTextGUI.text = $"{amount}";
                _bonusTextGUI.color = Color.red;
            }

            _bonusTextGUI.gameObject.SetActive(true);
            _bonusTextGUI.rectTransform.DOAnchorPosY(100, 0.5f).SetEase(Ease.OutQuad);

            _textGUI.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0), 0.5f, 10, 1);
            UpdateUI();
            yield return new WaitForSeconds(0.5f);
            _bonusTextGUI.gameObject.SetActive(false);

        }
    }
}
