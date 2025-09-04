
using System.Collections;
using System.Collections.Generic;
using System.Linq;


using UnityEngine;


namespace Quackery.Decks
{
    public class CardPilePool : MonoBehaviour
    {
        [Header("Card Pool Settings")]
        [SerializeField] private Transform _container;
        [SerializeField] private CardPile _cardPilePrefab;

        [SerializeField] protected EnumCardPile _cardPileType;
        protected readonly List<CardPile> _cardPiles = new();


        public IEnumerable<CardPile> OccupiedPiles => _cardPiles.Where(pile => pile.Enabled && !pile.IsEmpty);
        public IEnumerable<CardPile> EnabledPiles => _cardPiles.Where(pile => pile.Enabled);
        public IEnumerable<Card> TopCards => _cardPiles.Where(pile => pile.Enabled && !pile.IsEmpty)
                                                .Select(pile => pile.TopCard);

        public int EnabledCount => EnabledPiles.Count();

        protected virtual void OnCardPileTouchRelease(CardPile uI) { }
        protected virtual void OnCardPileTouchPress(CardPile uI) { }

        public IEnumerator SetPoolSize(int size)
        {
            if (EnabledCount == size) yield break;

            IncreasePoolTo(size);


            //Try to be nice and disable Empty Piles first
            for (int i = 0; i < _cardPiles.Count; i++)
            {
                if (EnabledCount <= size) continue;
                if (_cardPiles[i].IsEmpty && _cardPiles[i].Enabled)
                    _cardPiles[i].Enabled = false;
            }

            List<Card> cardToRemove = new();
            //If not disable Piles with content, starting from the last pile;
            for (int i = _cardPiles.Count - 1; i >= 0; i--)
            {
                if (EnabledCount <= size) yield break;
                if (_cardPiles[i].Enabled)
                {
                    yield return DeckServices.Discard(_cardPiles[i].RemoveAllCards());
                    _cardPiles[i].Enabled = false;
                }
            }
        }

        internal bool AddToEmptyPile(Card card, bool increaseIfFull)
        {
            var pile = GetEmptyPile(expandIfFull: increaseIfFull);
            if (pile == null) return false;
            pile.AddOnTop(card);
            return true;
        }

        protected void InstantiateNewPool(int diff)
        {
            for (int i = 0; i < diff; i++)
            {
                CardPile newCardPileUI = Instantiate(_cardPilePrefab, _container.transform);
                newCardPileUI.transform.localScale = Vector3.one;
                _cardPiles.Add(newCardPileUI);
                newCardPileUI.OnTouchPress += OnCardPileTouchPress;
                newCardPileUI.OnTouchRelease += OnCardPileTouchRelease;
                newCardPileUI.Type = _cardPileType;
            }
        }

        internal CardPile GetEmptyPile(bool expandIfFull)
        {
            foreach (var pile in _cardPiles)
                if (pile.Enabled && pile.IsEmpty) return pile;

            if (!expandIfFull) return null;

            IncreasePoolTo(EnabledCount + 1);

            foreach (var pile in _cardPiles)
                if (pile.Enabled && pile.IsEmpty) return pile;

            Debug.LogError("We should have reached this point", this);
            return null;
        }

        private void IncreasePoolTo(int size)
        {
            if (_cardPiles.Count < size)
                InstantiateNewPool(size - _cardPiles.Count);

            for (int i = 0; i < _cardPiles.Count; i++)
            {
                if (EnabledCount >= size) continue;
                if (!_cardPiles[i].Enabled)
                    _cardPiles[i].Enabled = true;
            }
        }

        internal void DisableEmptyPiles()
        {
            foreach (var pile in _cardPiles)
                if (pile.IsEmpty) pile.Enabled = false;
        }

        public virtual bool RemoveCard(Card card)
        {
            bool cardRemoved = false;
            foreach (var pile in _cardPiles)
                cardRemoved |= pile.RemoveCard(card);
            return cardRemoved;
        }


    }
}
