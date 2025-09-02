
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Quackery.Inventories;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Quackery.Decks
{

    public class CardPile : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] EnumCardPile _pileType;

        [SerializeField] protected bool _stripCard = false;

        protected readonly List<Card> _cards = new();
        public int PileIndex => transform.GetSiblingIndex();

        public bool IsEmpty => _cards.Count == 0;

        public Card TopCard => PeekTopCard();

        public EnumItemCategory Category => IsEmpty ? EnumItemCategory.Any : TopCard.Category;

        public int Count => _cards.Count;
        public bool Playable = true;
        public bool Enabled
        {
            get => gameObject.activeSelf;
            set => gameObject.SetActive(value);
        }


        public EnumCardPile Type
        {
            get => _pileType;
            set => _pileType = value;
        }

        public bool HasCartTarget => !IsEmpty && TopCard.HasCartTarget;

        public Vector2 AnchoredPosition
        {
            get => (transform as RectTransform).anchoredPosition;
            set => (transform as RectTransform).anchoredPosition = value;
        }

        protected readonly Queue<RectTransform> _moveQueue = new();

        public event Action OnCardMovedIn = delegate { };
        public event Action OnCardMovedOut = delegate { };
        public event Action OnPileUpdated = delegate { };

        public event Action<CardPile> OnTouchPress = delegate { };
        public event Action<CardPile> OnTouchRelease = delegate { };

        protected virtual void OnEnable()
        {

            DeckEvents.OnCardMovedTo += OnCardMoved;
            DeckEvents.OnShuffle += OnShuffle;

            DeckEvents.OnPileUpdated += OnPilesUpdated;

            StartCoroutine(StaggeredMoveRoutine());

        }

        protected virtual void OnDisable()
        {
            DeckEvents.OnCardMovedTo -= OnCardMoved;
            DeckEvents.OnShuffle -= OnShuffle;
            DeckEvents.OnPileUpdated -= OnPilesUpdated;


            StopAllCoroutines(); // Stop all coroutines when disabled
        }

        protected bool IsItMe(EnumCardPile type, int pileIndex)
        {
            return type == _pileType && pileIndex == PileIndex;
        }




        public void Shuffle() => _cards.Shuffle();

        public Card PeekTopCard() => IsEmpty ? null : _cards[0];

        public Card PeekBottomCard() => IsEmpty ? null : _cards[^1];


        public bool DrawTopCard(out Card card)
        {
            card = null;
            if (IsEmpty) return false;
            card = _cards[0];
            _cards.RemoveAt(0);
            return true;
        }

        public void Clear() => _cards.Clear();

        public bool RemoveCard(Card card) => _cards.Remove(card);


        internal void Add(Card card, EnumPlacement placement)
        {
            switch (placement)
            {
                case EnumPlacement.OnTop:
                    AddOnTop(card);
                    break;
                case EnumPlacement.AtTheBottom:
                    AddAtTheBottom(card);
                    break;
                case EnumPlacement.ShuffledIn:
                    AddShuffledIn(card);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(placement), placement, null);
            }
        }
        public void AddOnTop(Card card, bool isInstant = false)
        {
            if (card == null) return;
            _cards.Insert(0, card);
            MoveCardToPile(card, atTheTop: true, isInstant: isInstant);
        }
        public void AddAtTheBottom(Card card, bool isInstant = false)
        {
            if (card == null) return;
            _cards.Add(card);
            MoveCardToPile(card, atTheTop: false, isInstant: isInstant);
        }

        public void MergeOnTop(CardPile pile)
        {
            if (pile == null || pile.IsEmpty) return;

            foreach (var card in pile._cards)
            {
                if (card != null)
                {
                    AddOnTop(card);
                    //DeckEvents.OnCardMovedTo(card, Type, Index, true);
                }
            }
            pile.Clear();
        }

        internal void MergeBelow(CardPile pile)
        {
            if (pile == null || pile.IsEmpty) return;

            foreach (var card in pile._cards.ToArray())
            {
                if (card != null)
                {
                    AddAtTheBottom(card);
                    // DeckEvents.OnCardMovedTo(card, Type, Index, false);
                }
            }
            pile.Clear();
        }

        internal List<CardReward> CalculateCartRewards(List<CardPile> otherPiles)
        {
            if (IsEmpty || !Enabled) return new();
            var allCards = new List<Card>(_cards);
            allCards.Remove(TopCard);
            return TopCard.CalculateCardReward(allCards, otherPiles);

        }

        public void RestoreCategory()
        {
            foreach (var card in _cards)
                card.RemoveCategoryOverride();
        }

        internal void OverrideStackCategory(EnumItemCategory category)
        {
            for (int i = 0; i < _cards.Count; i++)
                _cards[i].OverrideCategory(category);
        }


        public bool Contains(Card card) => _cards.Contains(card);

        internal void UpdateUI()
        {
            if (IsEmpty || !Enabled) return;
            TopCard.UpdateUI();
        }

        private void AddShuffledIn(Card card, bool isInstant = false)
        {
            int index = UnityEngine.Random.Range(0, _cards.Count);
            _cards.Insert(index, card);
            MoveCardToPile(card, false, isInstant);
        }

        private void OnPilesUpdated(EnumCardPile type, int pileIndex)
        {
            if (!IsItMe(type, pileIndex)) return;
            OnPileUpdated?.Invoke();
        }

        public void SetActivated(bool activated)
        {
            TopCard.SetOutline(activated);
        }

        private void OnShuffle(EnumCardPile type, int pileIndex, List<Card> cards)
        {
            if (!IsItMe(type, pileIndex)) return;

            foreach (var card in GetComponentsInChildren<Card>())
            {
                if (!cards.Contains(card)) continue;
                card.transform.SetSiblingIndex(cards.IndexOf(card));
            }
        }

        private IEnumerator StaggeredMoveRoutine()
        {
            //var scaleRatio = Height / CardOriginalHeight;
            while (true)
            {
                if (_moveQueue.Count > 0)
                {
                    var cardTransform = _moveQueue.Dequeue();
                    if (cardTransform == null)
                        continue; // Skip if the card transform is null

                    cardTransform.DOScale(1, Tempo.WholeBeat).SetEase(Ease.Linear);
                    cardTransform.DOAnchorPos(Vector3.zero, Tempo.WholeBeat).SetEase(Ease.OutBack);
                    cardTransform.DOLocalRotate(Vector3.zero, Tempo.WholeBeat);

                    yield return new WaitForSeconds(Tempo.EighthBeat); // Stagger the movement of cards
                }
                else
                {
                    yield return null; // Wait for the next frame if no cards to move
                }
            }
        }

        private void OnCardMoved(Card card, EnumCardPile type, int pileIndex, bool atTheTop, bool isInstant)
        {
            if (!IsItMe(type, pileIndex)) return;

            MoveCardToPile(card, atTheTop, isInstant);
        }

        internal void MoveCardToPile(Card card, bool atTheTop, bool isInstant)
        {
            card.transform.SetParent(transform);
            card.transform.localScale = Vector3.one;
            card.Strip(_stripCard);
            if (atTheTop)
                card.transform.SetAsLastSibling();
            else
                card.transform.SetAsFirstSibling();

            if (isInstant)
            {
                card.RectTransform.anchoredPosition = Vector2.zero;
                card.transform.localRotation = Quaternion.identity;
                card.transform.localScale = Vector3.one;

                OnCardMovedIn?.Invoke();
            }
            else
                _moveQueue.Enqueue(card.transform as RectTransform);
            OnCardMovedIn?.Invoke();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnTouchPress?.Invoke(this);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            OnTouchRelease?.Invoke(this);
        }

        internal List<Card> RemoveAllCards()
        {
            List<Card> cards = new(_cards);
            _cards.RemoveAll(c => true);
            return cards;
        }
    }
}
