using System;
using System.Collections;
using System.Collections.Generic;
using Quackery.Decks;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Quackery
{
    public class CardTooltip : MonoBehaviour
    {
        [SerializeField] private LayoutGroup _layoutGroup;
        private CardPileUI _cardPileUI;

        private List<Tooltip> _tooltips = new();
        private bool _isOn;

        void Awake()
        {
            _cardPileUI = GetComponent<CardPileUI>();
            GetComponentsInChildren(true, _tooltips);
        }
        void OnEnable()
        {
            TooltipUI.ShowTooltipRequest += ShowTooltip;
            TooltipUI.HideTooltipRequest += Hide;
        }
        void OnDisable()
        {
            TooltipUI.ShowTooltipRequest -= ShowTooltip;
            TooltipUI.HideTooltipRequest -= Hide;
        }

        private void ShowTooltip(GameObject @object)
        {


            if (!@object.TryGetComponent<CardPileUI>(out CardPileUI cardPileUI) ||
                 _cardPileUI != cardPileUI || cardPileUI.IsEmpty || _isOn)
            {
                Hide();
                return;
            }

            bool isOnTheRight = cardPileUI.AnchoredPosition.x > Screen.width / 2;
            var rectTransform = _layoutGroup.transform as RectTransform;
            if (!isOnTheRight)
            {
                rectTransform.anchorMin = new Vector2(1, 1f);
                rectTransform.anchorMax = new Vector2(1, 1f);
                rectTransform.pivot = new Vector2(0, 1f);
                rectTransform.anchoredPosition = new Vector2(10, 0);
            }
            else
            {
                rectTransform.anchorMin = new Vector2(0, 1f);
                rectTransform.anchorMax = new Vector2(0, 1f);
                rectTransform.pivot = new Vector2(1, 1f);
                rectTransform.anchoredPosition = new Vector2(-10, 0);
            }


            _isOn = true;
            var explanations = cardPileUI.TopCard.Item.Data.Explanations;
            for (int i = 0; i < _tooltips.Count; i++)
            {
                if (i >= explanations.Count)
                {
                    _tooltips[i].Hide();
                    continue;
                }
                _tooltips[i].Show(Sprites.Replace(explanations[i].ShortDescription));
            }

            StartCoroutine(RebuildLayoutRoutine());

        }

        private IEnumerator RebuildLayoutRoutine()
        {
            yield return null;
            _layoutGroup.enabled = false;
            _layoutGroup.enabled = true;

            LayoutRebuilder.ForceRebuildLayoutImmediate(_layoutGroup.transform as RectTransform);

        }

        private void Hide()
        {
            _tooltips.ForEach(t => t.Hide());
            _isOn = false;
        }


    }
}
