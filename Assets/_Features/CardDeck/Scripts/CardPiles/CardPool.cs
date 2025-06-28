using System.Collections;
using System.Collections.Generic;
using Quackery.Decks;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Quackery
{
    public class CardPool : MonoBehaviour
    {
        [SerializeField] private GameObject _hiddable;
        [SerializeField] private Transform _container;
        [SerializeField] private EnumCardPile _cardPileType;
        [SerializeField] private CardPileUI _cardPilePrefab;
        private readonly List<CardPileUI> _cardPileUIs = new();

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
                    newCardPileUI.PileIndex = _cardPileUIs.Count - 1;
                    newCardPileUI.Type = _cardPileType;
                }
            }

            for (int i = 0; i < _cardPileUIs.Count; i++)
            {
                _cardPileUIs[i].gameObject.SetActive(i < newSize);
            }
        }

    }
}
