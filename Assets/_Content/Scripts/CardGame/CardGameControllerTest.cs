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

        // Start is called before the first frame update
        IEnumerator Start()
        {
            yield return FlowServices.WaitUntilEndOfSetup();
            yield return DeckServices.WaitUntilReady();
            yield return ClientServices.WaitUntilReady();
            yield return DialogServices.WaitUntilReady();

            CardGameState state = new(_controller);
            StartCoroutine(state.StateRoutine());
        }
    }
}
