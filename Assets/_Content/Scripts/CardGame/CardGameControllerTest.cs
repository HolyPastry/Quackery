using System;
using System.Collections;
using Quackery.Clients;
using Quackery.Decks;
using UnityEngine;

namespace Quackery
{
    public class CardGameControllerTest : MonoBehaviour
    {
        [SerializeField] private CardGameController _controller;

        // Start is called before the first frame update
        IEnumerator Start()
        {
            yield return FlowServices.WaitUntilEndOfSetup();
            yield return DeckServices.WaitUntilReady();
            StartCoroutine(Routine());
        }

        private IEnumerator Routine()
        {
            yield return null;
            var deckSetup = FindObjectOfType<SceneSetupDeck>();
            if (deckSetup != null)
                deckSetup.DrawCards();


            var client = ClientServices.GetNextClient();
            _controller.Show();


            yield return new WaitForSeconds(1f);
            while (true)
            {
                DeckServices.Shuffle();
                yield return new WaitForSeconds(1f);
                client = ClientServices.SelectedClient();

                _controller.StartNewRound(client);

                yield return _controller.WaitUntilEndOfRound();
                DeckServices.DiscardHand();
                if (_controller.RoundInterrupted)
                {
                    CartServices.SetCartValue(0);
                    client.BadReview();
                    ClientServices.ClientServed(client, false);
                }
                else
                {
                    _controller.TransfertCartToPurse();
                    yield return new WaitForSeconds(1f);
                    client.GoodReview();
                    ClientServices.ClientServed(client, true);
                }
                CartServices.DiscardCart();
                EffectServices.CleanEffects();
                _controller.ShowEndRoundScreen(!_controller.RoundInterrupted, out bool wasBoss);

                yield return new WaitForSeconds(5f);
                _controller.HideEndRoundScreen();

                yield return new WaitForSeconds(1f);
                if (ClientServices.HasNextClient())
                {
                    ClientServices.GetNextClient();
                }
                else
                {
                    _controller.ShowEndDayScreen();
                    yield return _controller.WaitUntilEndOfDayValidated();
                    ClientServices.ClientLeaves();
                    break;
                }
            }
        }
    }
}
