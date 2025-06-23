using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quackery
{
    [ExecuteAlways]
    public class RectMatchSize : MonoBehaviour
    {
        [SerializeField] private RectTransform _target;
        [SerializeField] private bool _matchWidth = true;
        [SerializeField] private bool _matchHeight = true;
        private RectTransform rectTransform => transform as RectTransform;

        // void OnValidate()
        // {
        //     MatchSize();
        // }

        IEnumerator Start()
        {

            yield return null;
            MatchSize();
        }

        private void MatchSize()
        {
            if (_target != null)
            {
                float width = _matchWidth
                    ? _target.rect.width - rectTransform.anchoredPosition.x / 2
                    : rectTransform.sizeDelta.x;

                float height = _matchHeight
                    ? _target.rect.height + rectTransform.anchoredPosition.y
                    : rectTransform.sizeDelta.y;

                rectTransform.sizeDelta = new Vector2(width, height);
            }
        }
    }
}
