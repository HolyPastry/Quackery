using UnityEngine;

namespace Quackery
{
    [ExecuteAlways]
    public class TeleportToRect : MonoBehaviour
    {
        [SerializeField] private RectTransform _targetRect;

        public void Teleport()
        {
            if (_targetRect != null)
            {
                transform.position = _targetRect.position;
            }
        }
        void Update()
        {
            Teleport();
        }
    }
}
