using System;
using System.Collections;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections.Generic;

namespace Quackery.TetrisBill
{
    [ExecuteAlways]
    public class TetrisCube : TetrisElement
    {
        private RectTransform _root;

        private Image _image;

        [SerializeField] private bool _isBorder = false;

        private TweenerCore<Color, Color, ColorOptions> _tween;

        public bool IsBorder => _isBorder;

        void Start()
        {

            _image = GetComponent<Image>();

        }

        public bool Overlaps(TetrisCube otherCube)
        {
            return RectTransformUtilityMethod(otherCube);
        }

        internal void FlashColor()
        {
            if (_image == null) _image = GetComponent<Image>();
            _tween = _image.DOColor(Color.black, 0.2f).SetLoops(-1, LoopType.Yoyo);
        }

        internal void Destroy()
        {
            _tween?.Kill();
            _tween = null;
            Destroy(gameObject);
        }

        private bool RectTransformUtilityMethod(TetrisCube other)
        {
            _root = _root != null ? _root : GetComponentInParent<Canvas>().transform as RectTransform;

            var boundsA = RectTransformUtility.CalculateRelativeRectTransformBounds(_root, this.rectTransform);
            var boundsB = RectTransformUtility.CalculateRelativeRectTransformBounds(_root, other.rectTransform);
            boundsA.size *= 0.9f; // Adjust size to avoid false positives due to pixel snapping

            return boundsA.Intersects(boundsB);
        }

        internal void MoveDownOne(float duration)
        {
            int newIndex = LineIndex - 1;
            if (newIndex < 0) return; // Prevent moving below the grid
            rectTransform.DOAnchorPosY(newIndex * TetrisGame.CellSize(), duration);
        }


        public void PlayDestroyEffect(float duration, Material material)
        {
            if (_image == null) _image = GetComponent<Image>();
            _image.material = material;
            _image.material.DOFloat(1, "_disolveAmount", duration);
        }
    }
}
