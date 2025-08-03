
using Holypastry.Bakery;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Quackery
{
    public class Anchor : Singleton<Anchor>
    {
        private Camera _mainCamera;

        [SerializeField] private InputActionReference _mousePositionAction;

        public Vector2 GetLocalMousePosition(RectTransform parent)
        {
            if (_mainCamera == null) _mainCamera = Camera.main;

            if (_mousePositionAction == null)
            {
                Debug.LogWarning("Anchor Object missing in the scene. Please add it to the scene.");
                return Vector2.zero;
            }

            Vector2 mousePosition = _mousePositionAction.action.ReadValue<Vector2>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parent,
                 mousePosition, _mainCamera, out Vector2 localPoint);
            return localPoint;
        }

        public Vector2 GetLocalAnchoredPosition(Vector3 worldPosition, RectTransform to)
        {
            if (_mainCamera == null) _mainCamera = Camera.main;
            var screenPoint = RectTransformUtility.WorldToScreenPoint(_mainCamera, worldPosition);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                to,
                 screenPoint, _mainCamera, out Vector2 localPoint);
            return localPoint;
        }
    }
}
