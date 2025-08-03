using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quackery
{
    public class DottedLine : MonoBehaviour
    {
        [SerializeField] private int _maxSegments = 8;
        [SerializeField] private RectTransform _dottedLinePrefab;
        public static Action<Vector3> GenerateDottedLine = (worldPosition) => { };
        public static Action HideDottedLine = delegate { };
        private readonly List<RectTransform> _dottedLineObjects = new();


        void OnEnable()
        {
            GenerateDottedLine += Generate;
            HideDottedLine += Hide;
        }


        void OnDisable()
        {
            GenerateDottedLine -= Generate;
            HideDottedLine -= Hide;

        }

        // private void MoveOnTopHandler(RectTransform transform, float duration, Action<RectTransform> action)
        // {
        //     StartCoroutine(MoveOnTopCoroutine(transform, duration, action));

        // }

        // private IEnumerator MoveOnTopCoroutine(RectTransform transform, float duration, Action<RectTransform> action)
        // {
        //     _previousParent = transform.parent;
        //     transform.SetParent(this.transform, false);
        //     transform.SetAsLastSibling();
        //     action?.Invoke(transform);
        //     yield return new WaitForSeconds(duration);
        //     transform.SetParent(_previousParent, false);
        // }

        private void Generate(Vector3 worldPosition)
        {
            var mousePosition = Anchor.Instance.GetLocalMousePosition(transform.parent as RectTransform);
            var originLocalPoint = Anchor.Instance.GetLocalAnchoredPosition(worldPosition,
                                            transform.parent as RectTransform);

            var dotWidth = _dottedLinePrefab.rect.width;

            // var direction = (mousePosition - originPosition).normalized;
            var distance = Vector3.Distance(mousePosition, originLocalPoint);
            var segments = Mathf.FloorToInt(distance / dotWidth);
            if (segments > _maxSegments)
            {
                segments = _maxSegments;
            }
            if (segments < 1)
            {
                Hide();
                return;
            }

            for (int i = 0; i < segments; i++)
            {
                if (_dottedLineObjects.Count > i)
                {
                    var dot = _dottedLineObjects[i];
                    dot.gameObject.SetActive(true);
                    dot.anchoredPosition = Vector3.Lerp(originLocalPoint, mousePosition, (float)i / segments);
                    continue;
                }
                else
                {
                    var dot = Instantiate(_dottedLinePrefab, transform);
                    dot.anchoredPosition = Vector3.Lerp(originLocalPoint, mousePosition, (float)i / segments);
                    _dottedLineObjects.Add(dot);
                }
            }
            // Disable extra dots
            for (int i = segments; i < _dottedLineObjects.Count; i++)
            {
                var dot = _dottedLineObjects[i];
                dot.gameObject.SetActive(false);
            }
        }

        private void Hide()
        {

            foreach (var dot in _dottedLineObjects)
            {
                dot.gameObject.SetActive(false);
            }
        }


    }
}
