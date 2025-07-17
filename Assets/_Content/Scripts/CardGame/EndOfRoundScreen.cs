using System;
using System.Collections;
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
        [SerializeField] private BossSuccessPanel _bossSuccessPanel;
        [SerializeField] private BossFailedPanel _bossFailedPanel;

        [SerializeField, Self] private AnimatedRect _animatedRect;
        private Client _revealedClient;
        private bool _clickPressed;

        public IEnumerator Show(Client client, bool success)
        {
            _revealedClient = null;
            _clickPressed = false;
            _knownClientPanel.Hide();
            _bossFailedPanel.Hide();
            _bossSuccessPanel.Hide();
            _anonymousClientPanel.Hide();

            gameObject.SetActive(true);
            _animatedRect.SlideIn(Direction.Right);
            yield return _animatedRect.WaitForAnimation();
            if (client.IsAnonymous)
            {
                _anonymousClientPanel.Show(client, success);
                _revealedClient = ClientServices.GetRevealedClient();
                if (_revealedClient == null) yield break;

                yield return new WaitUntil(() => _clickPressed);
                _knownClientPanel.Show(client, _revealedClient);
                _revealedClient = null;
                yield break;
            }

            if (success)
                _bossSuccessPanel.Show(client);
            else
                _bossFailedPanel.Show(client);
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
            if (_revealedClient != null)
                _clickPressed = true;
            else
                Hide(instant: false);
        }
    }
}
