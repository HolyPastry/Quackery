using TMPro;
using UnityEngine;

namespace Quackery
{
    [ExecuteAlways]
    public class RectCoordDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        private RectTransform _rectTransform => transform as RectTransform;

        void Update()
        {
            if (_text != null)
            {
                _text.text = $"{_rectTransform.anchoredPosition}\n {_rectTransform.position}";
            }
        }
    }
}
