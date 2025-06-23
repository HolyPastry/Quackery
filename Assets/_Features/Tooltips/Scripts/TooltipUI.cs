using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KBCore.Refs;
using TMPro;
using UnityEngine;

using UnityEngine.UI;

namespace Quackery
{

    public class TooltipUI : ValidatedMonoBehaviour
    {
        [SerializeField, Child] private TextMeshProUGUI _tooltipText;
        [SerializeField] private GameObject _hiddable;
        public static Action<GameObject> ShowTooltipRequest = delegate { };
        public static Action HideTooltipRequest = delegate { };
        private List<TooltipExtension> _extensions = new();

        void Awake()
        {
            GetComponents(_extensions);
            _hiddable.SetActive(false);
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
            _hiddable.SetActive(false);
            StopAllCoroutines();
        }

        internal void AddTooltip(string description)
        {
            _tooltipText.text = description;
            _hiddable.SetActive(true);

        }



    }
}
