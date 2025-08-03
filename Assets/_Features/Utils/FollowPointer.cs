
using System;
using KBCore.Refs;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Quackery
{


    public class FollowPointer : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private TextMeshProUGUI _text;

        [SerializeField] private RectTransform _pointer;

        [SerializeField] private RectTransform _otherThing;

        public RectTransform RectTransform => transform as RectTransform;

        public Vector2 AnchoredPosition
        {
            get => RectTransform.anchoredPosition;
            set => RectTransform.anchoredPosition = value;
        }

        private void Update()
        {

            _pointer.anchoredPosition =
                Anchor.Instance.GetLocalMousePosition(_pointer.parent as RectTransform);

            var localPosition = Anchor.Instance.GetLocalAnchoredPosition(_otherThing.position, this.RectTransform);
            _text.text = $"Local Position:\n{localPosition}";

            DottedLine.GenerateDottedLine(_otherThing.position);

        }
    }
}
