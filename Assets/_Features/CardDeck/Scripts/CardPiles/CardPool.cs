using System;
using System.Collections;
using System.Collections.Generic;
using Quackery.Decks;
using UnityEngine;

namespace Quackery
{
    public class CardPool : MonoBehaviour
    {
        [SerializeField] private GameObject _hiddable;
        [SerializeField] private Transform _container;
        [SerializeField] private EnumCardPile _cardPileType;
        [SerializeField] private CardPileUI _cardPilePrefab;
        protected readonly List<CardPileUI> _cardPileUIs = new();

        void OnEnable()
        {
            DeckEvents.OnCardPoolSizeUpdate += UpdatePoolSize;
        }

        void OnDisable()
        {
            DeckEvents.OnCardPoolSizeUpdate -= UpdatePoolSize;
        }

        IEnumerator Start()
        {
            yield return FlowServices.WaitUntilReady();
            yield return DeckServices.WaitUntilReady();
            UpdatePoolSize(_cardPileType);
        }

        public void Show()
        {
            UpdatePoolSize(_cardPileType);
            _hiddable.SetActive(true);
        }

        public void Hide()
        {
            _hiddable.SetActive(false);
        }

        private void UpdatePoolSize(EnumCardPile cardPile)
        {
            if (cardPile != _cardPileType) return;
            int newSize = DeckServices.GetCardPoolSize(_cardPileType);
            if (newSize > _cardPileUIs.Count)
            {
                int diff = newSize - _cardPileUIs.Count;
                for (int i = 0; i < diff; i++)
                {
                    CardPileUI newCardPileUI = Instantiate(_cardPilePrefab, _container.transform);
                    _cardPileUIs.Add(newCardPileUI);
                    newCardPileUI.OnCardPressed += BringToFront;
                    newCardPileUI.PileIndex = _cardPileUIs.Count - 1;
                    newCardPileUI.Type = _cardPileType;
                }
            }
            while (newSize < _cardPileUIs.Count)
            {
                DestroyCardPile(_cardPileUIs.Count - 1);
            }
        }

        protected virtual void DestroyCardPile(int index)
        {
            CardPileUI lastCardPileUI = _cardPileUIs[index];

            lastCardPileUI.OnCardPressed -= BringToFront;
            _cardPileUIs.RemoveAt(index);
            Destroy(lastCardPileUI.gameObject);
        }

        protected virtual void BringToFront(CardPileUI pileUI)
        {

        }
    }
}
