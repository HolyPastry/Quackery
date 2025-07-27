using System;
using System.Collections;
using System.Collections.Generic;
using Quackery.Decks;
using UnityEngine;

namespace Quackery
{
    public class CardPool : MonoBehaviour
    {
        [Header("Card Pool Settings")]
        [SerializeField] private GameObject _hiddable;
        [SerializeField] private Transform _container;
        [SerializeField] private EnumCardPile _cardPileType;
        [SerializeField] private CardPileUI _cardPilePrefab;
        protected readonly List<CardPileUI> _cardPileUIs = new();

        protected virtual void OnEnable()
        {
            //DeckEvents.OnCardPoolSizeUpdate += UpdatePoolSize;
            DeckEvents.OnCardPoolSizeIncrease += IncreasePoolSize;
            DeckEvents.OnCardPoolSizeDecrease += DecreasePoolSize;

        }

        protected virtual void OnDisable()
        {
            //DeckEvents.OnCardPoolSizeUpdate -= UpdatePoolSize;
            DeckEvents.OnCardPoolSizeIncrease -= IncreasePoolSize;
            DeckEvents.OnCardPoolSizeDecrease -= DecreasePoolSize;
        }

        protected void DecreasePoolSize(EnumCardPile pile, int index)
        {
            if (pile != _cardPileType) return;
            DestroyCardPile(index);

        }

        protected void IncreasePoolSize(EnumCardPile pile)
        {
            if (pile != _cardPileType) return;
            int newSize = DeckServices.GetCardPoolSize(_cardPileType);

            int diff = newSize - _cardPileUIs.Count;
            for (int i = 0; i < diff; i++)
            {
                CardPileUI newCardPileUI = Instantiate(_cardPilePrefab, _container.transform);
                _cardPileUIs.Add(newCardPileUI);
                newCardPileUI.OnTouchPress += OnCardPileTouchPress;
                newCardPileUI.OnTouchRelease += OnCardPileTouchRelease;

                newCardPileUI.PileIndex = _cardPileUIs.Count - 1;
                newCardPileUI.Type = _cardPileType;
            }
        }

        IEnumerator Start()
        {
            yield return FlowServices.WaitUntilReady();
            yield return DeckServices.WaitUntilReady();
            IncreasePoolSize(_cardPileType);
        }

        public void Show()
        {
            //UpdatePoolSize(_cardPileType);
            _hiddable.SetActive(true);
        }

        public void Hide()
        {
            _hiddable.SetActive(false);
        }


        protected virtual void DestroyCardPile(int index)
        {
            CardPileUI cardPileUI = _cardPileUIs.Find(pile => pile.PileIndex == index);

            if (cardPileUI == null)
            {
                Debug.LogWarning($"CardPileUI with index {index} not found in {_cardPileType} pool.");
                return;
            }

            cardPileUI.OnTouchPress -= OnCardPileTouchPress;
            cardPileUI.OnTouchRelease -= OnCardPileTouchRelease;

            _cardPileUIs.Remove(cardPileUI);
            Destroy(cardPileUI.gameObject);
        }

        protected virtual void OnCardPileTouchRelease(CardPileUI uI) { }

        protected virtual void OnCardPileTouchPress(CardPileUI uI) { }
    }
}
