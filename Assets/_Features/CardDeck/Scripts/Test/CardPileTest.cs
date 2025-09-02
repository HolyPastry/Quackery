using System.Collections;
using System.Collections.Generic;
using Quackery.Decks;
using UnityEngine;

namespace Quackery
{
    public class CardPileTest : MonoBehaviour
    {
        [SerializeField] private GameObject _object;
        [SerializeField] private float _delay;

        [SerializeField] HandController _handController;
        IEnumerator Start()
        {
            yield return new WaitForSeconds(_delay);
            _object.SetActive(true);

            CartServices.InitCart();

            yield return DeckServices.DrawBackToFull();

            yield return DeckServices.DiscardHand();

            yield return _handController.SetPoolSize(5);

            yield return DeckServices.DrawBackToFull();
            yield return DeckServices.DiscardHand();

            yield return _handController.SetPoolSize(3);

            yield return DeckServices.DrawBackToFull();

            yield return DeckServices.DiscardHand();
            yield return _handController.SetPoolSize(6);

            yield return DeckServices.DrawBackToFull();

            yield return _handController.SetPoolSize(3);


            DeckServices.ActivateHand();


        }
    }
}
