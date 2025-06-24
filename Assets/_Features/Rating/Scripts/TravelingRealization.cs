using DG.Tweening;
using UnityEngine;

namespace Quackery
{
    public class TravelingRealization : MonoBehaviour
    {
        [SerializeField] private Transform _travelingObject;
        [SerializeField] private float _travelDuration = 1f;
        [SerializeField] private Ease _travelEase = Ease.Linear;
        [SerializeField] private Ease _scaleEase = Ease.OutBack;
        [SerializeField] private float _scaleFactor = 1.5f;

        private Vector3 _originalPosition;
        private float _originalRotation;

        public void Awake()
        {
            _originalPosition = _travelingObject.position;
            _originalRotation = _travelingObject.localEulerAngles.z;
        }
        public void ResetPosition()
        {
            _travelingObject.gameObject.SetActive(false);
            _travelingObject.position = _originalPosition;
            _travelingObject.localEulerAngles = new Vector3(0, 0, _originalRotation);

        }
        public void Travel()
        {
            _travelingObject.gameObject.SetActive(true);
            _travelingObject.DOMove(transform.position, _travelDuration)
                .SetEase(_travelEase);
            _travelingObject.DOScale(_scaleFactor * Vector3.one, _travelDuration)
                .SetEase(_scaleEase);
        }

    }
}
