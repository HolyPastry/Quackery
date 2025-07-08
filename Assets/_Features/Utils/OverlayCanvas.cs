using System;
using System.Collections.Generic;
using UnityEngine;

namespace Quackery
{
    public class OverlayCanvas : MonoBehaviour
    {
        [SerializeField] private int _maxSegments = 8;
        [SerializeField] private RectTransform _dottedLinePrefab;
        public static Action<Vector3, Vector3> GenerateDottedLine = delegate { };
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

        private void Generate(Vector3 originPosition, Vector3 mousePosition)
        {
            Debug.Log($"Generating dotted line from {originPosition} to {mousePosition}");
            var dotWidth = _dottedLinePrefab.rect.width;

            // var direction = (mousePosition - originPosition).normalized;
            var distance = Vector3.Distance(mousePosition, originPosition);
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
                    dot.position = Vector3.Lerp(originPosition, mousePosition, (float)i / segments);
                    continue;
                }
                else
                {
                    var dot = Instantiate(_dottedLinePrefab);
                    dot.transform.SetParent(transform, false);
                    dot.position = Vector3.Lerp(originPosition, mousePosition, (float)i / segments);
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
