using System;
using System.Collections;
using System.Collections.Generic;
using KBCore.Refs;

using UnityEngine;

using UnityEngine.UI;


namespace Quackery
{
    public class Tooltips : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private LayoutGroup _layoutGroup;
        [SerializeField] private int _offset = 100;

        public static Action<ITooltipTarget> ShowTooltipRequest = delegate { };
        public static Action HideTooltipRequest = delegate { };

        private RectTransform _rectTransform => transform as RectTransform;
        private ITooltipTarget _tooltipTarget;

        private readonly List<Tooltip> _tooltips = new();
        protected bool _isOn;

        void Awake()
        {
            GetComponentsInChildren(true, _tooltips);
            _tooltipTarget = GetComponentInParent<ITooltipTarget>();
            if (_tooltipTarget == null)
            {
                Debug.LogWarning($"No ITooltipTarget found in parent hierarchy.{transform.parent.name}", this);
                return;
            }
            foreach (var tooltip in _tooltips)
            {
                tooltip.Hide();
            }
        }
        void OnEnable()
        {
            Tooltips.ShowTooltipRequest += ShowTooltip;
            Tooltips.HideTooltipRequest += Hide;
        }
        void OnDisable()
        {
            Tooltips.ShowTooltipRequest -= ShowTooltip;

            Tooltips.HideTooltipRequest -= Hide;
        }


        protected void ShowTooltip(ITooltipTarget tooltipTarget)
        {
            if (tooltipTarget == null ||
                tooltipTarget != _tooltipTarget ||
                tooltipTarget.RectTransform == null ||
                _isOn
                )
            {
                Hide();
                return;
            }
            _isOn = true;
            FillExplanations(tooltipTarget.Explanations);
            Position(tooltipTarget.RectTransform);

        }

        protected void Hide()
        {
            StopAllCoroutines();
            _tooltips.ForEach(t => t.Hide());
            _isOn = false;
        }
        protected void Position(RectTransform targetRectTransform)
        {
            bool isOnTheRight = targetRectTransform.position.x > 0;

            if (!isOnTheRight)
            {
                _rectTransform.anchorMin = new Vector2(1, 1f);
                _rectTransform.anchorMax = new Vector2(1, 1f);
                _rectTransform.pivot = new Vector2(0, 1f);
                _rectTransform.anchoredPosition = new Vector2(-_offset, 0);
            }
            else
            {
                _rectTransform.anchorMin = new Vector2(0, 1f);
                _rectTransform.anchorMax = new Vector2(0, 1f);
                _rectTransform.pivot = new Vector2(1, 1f);
                _rectTransform.anchoredPosition = new Vector2(_offset, 0);
            }
            StartCoroutine(ForceRebuildLayout(targetRectTransform));
        }

        private IEnumerator ForceRebuildLayout(RectTransform targetRectTransform)
        {
            yield return null;
            _layoutGroup.enabled = false;
            _layoutGroup.enabled = true;

            LayoutRebuilder.ForceRebuildLayoutImmediate(_layoutGroup.transform as RectTransform);
            yield return null;
        }

        protected void FillExplanations(List<Explanation> explanations)
        {
            if (_tooltips.Count == 0)
                GetComponentsInChildren(true, _tooltips);

            for (int i = 0; i < _tooltips.Count; i++)
            {
                if (i >= explanations.Count)
                {
                    _tooltips[i].Hide();
                    continue;
                }
                _tooltips[i].Show(Sprites.Replace(explanations[i].ShortDescription));
            }
            Debug.Log(_tooltips[0].gameObject.activeSelf);
        }


    }
}
