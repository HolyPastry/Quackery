using System;

using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery
{

    public class TooltipUI : MonoBehaviour
    {
        public static Action<GameObject> ShowTooltipRequest = delegate { };
        public static Action HideTooltipRequest = delegate { };

        // public static Action<GameObject, List<Explanation>> ShowTooltip = delegate { };
        // private List<TooltipExtension> _extensions = new();
        // private List<Tooltip> _tooltips = new();

        // private Canvas _root;

        // void Awake()
        // {
        //     GetComponentsInChildren(true, _tooltips);
        //     GetComponents(_extensions);

        //     _tooltips.ForEach(t => t.Hide());

        //     _root = GetComponentInParent<Canvas>();

        // }

        // void OnEnable()
        // {
        //     ShowTooltipRequest = ShowTooltip;
        //     HideTooltipRequest = HideTooltip;
        // }
        // void OnDisable()
        // {
        //     ShowTooltipRequest = delegate { };
        //     HideTooltipRequest = delegate { };
        // }

        // private void ShowTooltip(GameObject hoveredObject)
        // {
        //     foreach (var extension in _extensions)
        //         extension.SetTooltip(hoveredObject);

        // }

        // private void HideTooltip()
        // {
        //     _tooltips.ForEach(t => t.Hide());
        //     StopAllCoroutines();
        // }

        // internal void AddTooltip(RectTransform rectTransform, List<Explanation> explanations)
        // {
        //     // _descriptionGUI.text = Sprites.Replace(description);
        //     // _titleGUI.text = title;
        //     Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(_root.transform, rectTransform);
        //     for (int i = 0; i < _tooltips.Count; i++)
        //     {

        //         if (i >= explanations.Count)
        //         {
        //             _tooltips[i].Hide();
        //             continue;
        //         }
        //         _tooltips[i].Show(Sprites.Replace(explanations[i].ShortDescription));
        //         _tooltips[i].AnchoredPosition = bounds.max;
        //     }

        //     OnShowTooltip.Invoke(null);
        //     //  LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        // }



    }
}
