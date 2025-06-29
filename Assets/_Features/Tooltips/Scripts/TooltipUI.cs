using System;

using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery
{

    public class TooltipUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _descriptionGUI;
        [SerializeField] private TextMeshProUGUI _titleGUI;
        [SerializeField] private TextMeshProUGUI _explanationGUI;
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

        internal void AddTooltip(string title, string description, List<Explanation> explanation)
        {
            _descriptionGUI.text = Sprites.Replace(description);
            _titleGUI.text = title;

            _explanationGUI.gameObject.SetActive(explanation.Count > 0);
            if (explanation.Count > 0)
            {
                _explanationGUI.text = "";
                foreach (var exp in explanation)
                {
                    _explanationGUI.text += Sprites.Replace(exp.ShortDescription) + "\n";
                }
            }
            _hiddable.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_hiddable.transform as RectTransform);

        }



    }
}
