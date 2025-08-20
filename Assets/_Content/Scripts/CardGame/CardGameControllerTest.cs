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
        [SerializeField] private CardGameApp _controller;
        [SerializeField] private float _timeScale = 1f;

        // Start is called before the first frame update
        IEnumerator Start()
        {
            yield return FlowServices.WaitUntilEndOfSetup();
            // yield return DeckServices.WaitUntilReady();
            yield return ClientServices.WaitUntilReady();
            yield return DialogServices.WaitUntilReady();

            //  Time.timeScale = _timeScale;
            CardGameState state = new(_controller);
            yield return new WaitForSeconds(1f);
            StartCoroutine(state.StateRoutine());
        }
    }
}
