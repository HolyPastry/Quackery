using Holypastry.Bakery;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Quackery.Decks
{
    public class ShowTooltip : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private float _holdingTime = 0.4f;

        CountdownTimer _holdingTimer;

        void Awake()
        {
            _holdingTimer = new CountdownTimer(_holdingTime);
        }

        void OnEnable()
        {
            _holdingTimer.OnTimerEnd += SendTooltipRequest;
        }

        void OnDisable()
        {
            _holdingTimer.OnTimerEnd -= SendTooltipRequest;
            _holdingTimer.Stop();
        }
        void Update()
        {
            _holdingTimer.Tick(Time.deltaTime);
        }
        private void SendTooltipRequest()
        {
            TooltipUI.ShowTooltipRequest(gameObject);
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            if (_holdingTimer.IsRunning) return;
            _holdingTimer.Start();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _holdingTimer.Stop();
            TooltipUI.HideTooltipRequest();
        }
    }
}
