using KBCore.Refs;
using Quackery.Clients;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Quackery
{
    public class EndOfRoundScreen : ValidatedMonoBehaviour, IPointerDownHandler
    {

        [SerializeField] private AnonymousClientPanel _anonymousClientPanel;
        [SerializeField] private KnownClientPanel _knownClientPanel;
        [SerializeField, Self] private AnimatedRect _animatedRect;
        public void Show(Client client, bool success)
        {
            gameObject.SetActive(true);
            if (ClientServices.IsCurrentClientAnonymous())
            {
                _knownClientPanel.Hide();
                _anonymousClientPanel.Show(client, success);
            }
            else
            {
                _anonymousClientPanel.Hide();
                _knownClientPanel.Show(client, ClientServices.GetRevealedClient());
            }

            _animatedRect.SlideIn(Direction.Right);
        }

        public void Hide(bool instant)
        {
            if (instant)
                gameObject.SetActive(false);
            else
                _animatedRect.ZoomOut(false)
                              .DoComplete(() => gameObject.SetActive(false));
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Hide(instant: false);
        }
    }
}
