using System;
using System.Collections;
using System.Collections.Generic;
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
            StartCoroutine(Routine());
        }

        private IEnumerator Routine()
        {
            var client = ClientServices.GetNextClient();
            _controller.Show();
            yield return new WaitForSeconds(1f);
            while (true)
            {
                client = ClientServices.SelectedClient();

                _controller.StartNewRound(client);

                yield return _controller.WaitUntilEndOfRound();

                _controller.ApplyEndRoundEffects();


                EffectServices.CleanEffects();
                if (_controller.RoundInterrupted)
                {
                    client.BadReview();
                    ClientServices.ClientServed(client.Data);
                }
                else
                {
                    _controller.TransfertCartToPurse();
                    yield return new WaitForSeconds(1f);
                    client.GoodReview();
                    ClientServices.ClientServed(client.Data);
                }
                _controller.ResetDeck();
                EffectServices.CleanEffects();
                _controller.ShowEndRoundScreen(!_controller.RoundInterrupted);

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
