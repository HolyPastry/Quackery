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
        [SerializeField] private int _offset = 100;

        public static Action<ITooltipTarget> ShowTooltipRequest = delegate { };
        public static Action HideTooltipRequest = delegate { };
        [SerializeField, Self] private LayoutGroup _layoutGroup;
        private RectTransform rectTransform => transform as RectTransform;
        private ITooltipTarget _tooltipTarget;

        private List<Tooltip> _tooltips = new();
        protected bool _isOn;

        void Awake()
        {
            GetComponentsInChildren(true, _tooltips);
            _tooltipTarget = GetComponentInParent<ITooltipTarget>();
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
            StartCoroutine(PositionRoutine(targetRectTransform));
        }

        private IEnumerator PositionRoutine(RectTransform targetRectTransform)
        {
            bool isOnTheRight = targetRectTransform.position.x > Screen.width / 2;
            if (!isOnTheRight)
            {
                rectTransform.anchorMin = new Vector2(1, 1f);
                rectTransform.anchorMax = new Vector2(1, 1f);
                rectTransform.pivot = new Vector2(0, 1f);
                rectTransform.anchoredPosition = new Vector2(-_offset, 0);
            }
            else
            {
                rectTransform.anchorMin = new Vector2(0, 1f);
                rectTransform.anchorMax = new Vector2(0, 1f);
                rectTransform.pivot = new Vector2(1, 1f);
                rectTransform.anchoredPosition = new Vector2(_offset, 0);
            }

            //  rectTransform.localEulerAngles = new Vector3(0, 0, -targetRectTransform.rotation.eulerAngles.z);

            yield return null;
            _layoutGroup.enabled = false;
            _layoutGroup.enabled = true;

            LayoutRebuilder.ForceRebuildLayoutImmediate(_layoutGroup.transform as RectTransform);
            yield return null;
        }





        protected void FillExplanations(List<Explanation> explanations)
        {
            for (int i = 0; i < _tooltips.Count; i++)
            {
                if (i >= explanations.Count)
                {
                    _tooltips[i].Hide();
                    continue;
                }
                _tooltips[i].Show(Sprites.Replace(explanations[i].ShortDescription));
            }
        }


    }
}
