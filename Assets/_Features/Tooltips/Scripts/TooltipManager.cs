using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Quackery
{

    public class TooltipManager : MonoBehaviour
    {
        [SerializeField] private InputActionReference _mousePoint;
        public static Action<GameObject> ShowTooltipRequest = delegate { };
        public static Action HideTooltipRequest = delegate { };

        private List<TooltipExtension> _extensions = new();
        private List<Tooltip> _tooltipPool = new();

        void Awake()
        {
            GetComponents(_extensions);
            GetComponentsInChildren(true, _tooltipPool);
        }

        void OnEnable()
        {
            ShowTooltipRequest = ShowTooltip;
            HideTooltipRequest = HideTooltip;
        }
        void OnDisable()
        {
            ShowTooltipRequest = delegate { };
            HideTooltipRequest = delegate { };
        }

        private void ShowTooltip(GameObject hoveredObject)
        {
            foreach (var extension in _extensions)
                extension.SetTooltip(hoveredObject);


        }



        private void HideTooltip()
        {
            foreach (var tooltip in _tooltipPool)
                tooltip.Hide();
            StopAllCoroutines();
        }

        internal void AddTooltip(string description)
        {
            var tooltip = _tooltipPool.FirstOrDefault(t => !t.gameObject.activeSelf);
            if (tooltip != null) tooltip.Show(description);
        }

        internal void RefreshLayout()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        }

        internal void AddTooltip(object multiplierText)
        {
            throw new NotImplementedException();
        }
    }
}
