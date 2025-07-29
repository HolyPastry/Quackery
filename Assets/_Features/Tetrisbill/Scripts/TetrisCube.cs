using System;
using System.Collections;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

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


        public bool Collided { get; private set; }

        public void OnTriggerEnter2D(Collider2D other)
        {
            Collided = true;
        }
        public void OnTriggerExit2D(Collider2D other)
        {
            Collided = false;
        }

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
            _tween = _image.DOColor(Color.black, 0.2f).SetLoops(-1, LoopType.Yoyo);
        }

        internal void Destroy()
        {
            _tween?.Kill();
            _tween = null;
            Destroy(gameObject);
        }

        internal void MoveY(int v, float duration)
        {
            rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y + v, duration);
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

        internal void Clear()
        {
            throw new NotImplementedException();
        }
    }
}
