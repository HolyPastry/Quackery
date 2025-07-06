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

        public bool WasBoss { get; private set; }
        public IEnumerator Show(Client client, bool success)
        {
            _knownClientPanel.Hide();
            _bossFailedPanel.Hide();
            _bossSuccessPanel.Hide();
            _anonymousClientPanel.Hide();
            WasBoss = false;
            gameObject.SetActive(true);
            _animatedRect.SlideIn(Direction.Right);
            yield return _animatedRect.WaitForAnimation();
            if (ClientServices.IsCurrentClientAnonymous())
            {
                _anonymousClientPanel.Show(client, success);
                yield break;
            }
            var revealedClient = ClientServices.GetRevealedClient();
            if (revealedClient != null)
            {

                _knownClientPanel.Show(client, revealedClient);

                yield break;
            }
            WasBoss = true;

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
            Hide(instant: false);
        }
    }
}
