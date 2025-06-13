
using DG.Tweening;
using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Quackery
{
    public class StatusUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _valueText;
        [SerializeField] private GameObject _shortBg;
        [SerializeField] private GameObject _longBg;
        public Status Status { get; private set; }

        public void OnPointerEnter(PointerEventData eventData)
        {
            TooltipManager.ShowTooltipRequest.Invoke(gameObject);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TooltipManager.HideTooltipRequest.Invoke();
        }

        internal void UpdateStatus(Status status, Vector2 originPosition)
        {
            UpdateStatus(status);
            MakeIconFlyToPosition(originPosition);
        }

        internal void UpdateStatus(Status status)
        {
            if (status.Data == null)
            {
                Debug.LogWarning("StatusData is null. Cannot update StatusUI.");
                return;
            }

            Status = status;
            _icon.sprite = status.Data.Icon;

            if (status.Data.UseValue == false)
            {
                _valueText.text = string.Empty;
                _shortBg.SetActive(true);
                _longBg.SetActive(false);
            }
            else
            {
                _valueText.text = status.Value.ToString();
                _shortBg.SetActive(false);
                _longBg.SetActive(true);
            }
            gameObject.SetActive(true);
        }

        private void MakeIconFlyToPosition(Vector2 originPosition)
        {
            var originalPosition = _icon.transform.position;
            _icon.transform.position = originPosition;
            _icon.transform.localScale = Vector3.zero;
            _icon.transform.DOMove(originalPosition, 0.5f);
            _icon.transform.DOScale(Vector3.one, 0.5f)
                .SetEase(Ease.OutBack)
                .OnComplete(() => _icon.transform.localScale = Vector3.one);
        }
    }
}
