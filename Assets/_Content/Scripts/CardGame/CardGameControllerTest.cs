using System;
using System.Collections;
using Quackery.Clients;
using Quackery.Decks;
using UnityEngine;
using UnityEngine.Android;

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

            //TODO:: Find a way to make this work in a generic fashion
            var deckSetup = FindObjectOfType<SceneSetupDeck>();
            if (deckSetup != null)
                deckSetup.DrawCards();

            DeckServices.Shuffle();
            deckSetup.AddOnTopOfDeck();
            // ------------------------------------------------------

            bool firstTime = true;

            var client = ClientServices.GetNextClient();
            _controller.Show();


            yield return new WaitForSeconds(1f);
            while (true)
            {
                if (firstTime)
                    firstTime = false;
                else DeckServices.Shuffle();

                yield return new WaitForSeconds(1f);
                client = ClientServices.SelectedClient();

                _controller.StartNewRound(client);

                yield return _controller.WaitUntilEndOfRound();
                DeckServices.DiscardHand();
                if (_controller.RoundInterrupted)
                {
                    CartServices.ResetCartValue();
                    client.BadReview();
                    ClientServices.ClientServed(client, false);
                }
                else
                {
                    yield return new WaitForSeconds(1f);
                    client.GoodReview();
                    ClientServices.ClientServed(client, true);
                }

                _controller.ShowEndRoundScreen(!_controller.RoundInterrupted, out bool wasBoss);
                yield return new WaitForSeconds(3f);
                _controller.HideEndRoundScreen();
                _controller.TransfertCartToPurse();
                CartServices.DiscardCart();
                EffectServices.CleanEffects();

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
