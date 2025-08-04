
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Quackery.Decks
{

    public class CardPileUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] EnumCardPile _pileType;
        [SerializeField] protected float _moveSpeed = 0.5f;
        [SerializeField] protected Ease _easeType = Ease.OutBack;
        [SerializeField] protected float _staggerDelay = 0.1f;
        [SerializeField] protected Transform _cardParent;

        [SerializeField] protected bool _stripCard = false;



        public int PileIndex { get; set; } = 0;


        public EnumCardPile Type
        {
            get => _pileType;
            set => _pileType = value;

        }
        public bool IsEmpty => GetComponentsInChildren<Card>().Length == 0;
        public Card TopCard => IsEmpty ? null : GetComponentsInChildren<Card>()[^1];

        public float Height => (transform as RectTransform).sizeDelta.y;

        public bool HasCartTarget => !IsEmpty && TopCard.HasCartTarget;

        public Vector2 AnchoredPosition
        {
            get => (transform as RectTransform).anchoredPosition;
            set => (transform as RectTransform).anchoredPosition = value;
        }


        public const float CardOriginalHeight = 512;
        protected readonly Queue<RectTransform> _moveQueue = new();

        public event Action OnCardMovedIn = delegate { };
        public event Action OnCardMovedOut = delegate { };
        public event Action OnPileUpdated = delegate { };

        public event Action<CardPileUI> OnTouchPress = delegate { };
        public event Action<CardPileUI> OnTouchRelease = delegate { };

        protected virtual void OnEnable()
        {
            if (_cardParent == null)
                _cardParent = transform;
            DeckEvents.OnCardMovedTo += OnCardMoved;
            DeckEvents.OnShuffle += OnShuffle;
            DeckEvents.OnPileDestroyed += OnPileDestroyed;
            DeckEvents.OnActivatePile += OnActivatePile;

            DeckEvents.OnPileUpdated += OnPilesUpdated;

            StartCoroutine(StaggeredMoveRoutine());

        }

        protected virtual void OnDisable()
        {
            DeckEvents.OnCardMovedTo -= OnCardMoved;
            DeckEvents.OnShuffle -= OnShuffle;
            DeckEvents.OnPileDestroyed -= OnPileDestroyed;
            DeckEvents.OnActivatePile -= OnActivatePile;
            DeckEvents.OnPileUpdated -= OnPilesUpdated;


            StopAllCoroutines(); // Stop all coroutines when disabled
        }

        protected bool IsItMe(EnumCardPile type, int pileIndex)
        {
            return type == _pileType && pileIndex == PileIndex;
        }

        private void OnPilesUpdated(EnumCardPile type, int pileIndex)
        {
            if (!IsItMe(type, pileIndex)) return;
            OnPileUpdated?.Invoke();

        }

        private void OnActivatePile(EnumCardPile type, int pileIndex, bool activated)
        {
            if (!IsItMe(type, pileIndex) || IsEmpty) return;

            TopCard.SetOutline(activated);
        }


        private void OnPileDestroyed(EnumCardPile type, int pileIndex)
        {
            if (!IsItMe(type, pileIndex)) return;

            DestroyCards();
            OnCardMovedOut?.Invoke();

        }

        private void OnShuffle(EnumCardPile type, int pileIndex, List<Card> cards)
        {
            if (!IsItMe(type, pileIndex)) return;

            foreach (var card in GetComponentsInChildren<Card>())
            {
                if (!cards.Contains(card)) continue;
                card.transform.SetSiblingIndex(cards.IndexOf(card));
            }
            // StartCoroutine(ShuffleCardRoutine(cards));
        }

        private IEnumerator ShuffleCardRoutine(List<Card> cards)
        {

            foreach (var card in GetComponentsInChildren<Card>())
            {
                if (!cards.Contains(card)) continue;
                var rectTransform = card.transform as RectTransform;

                rectTransform.DOLocalMoveX(-rectTransform.rect.width, 0.01f)
                    .SetEase(_easeType)
                    .OnComplete(() =>
                    {
                        transform.SetSiblingIndex(cards.IndexOf(card));
                        rectTransform.DOLocalMoveX(0, _moveSpeed)
                            .SetEase(_easeType)
                            .OnComplete(() =>
                            {

                            });
                    });
                yield return new WaitForSeconds(_staggerDelay); // Stagger the movement of cards
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
                    cardTransform.DOScale(1, _moveSpeed).SetEase(Ease.Linear);
                    cardTransform.DOAnchorPos(Vector3.zero, _moveSpeed).SetEase(_easeType);
                    cardTransform.DOLocalRotate(Vector3.zero, _moveSpeed);


                    yield return new WaitForSeconds(_staggerDelay); // Stagger the movement of cards
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
            card.transform.SetParent(_cardParent, false);
            card.transform.localScale = Vector3.one;
            card.Strip(_stripCard);

            if (atTheTop)
            {
                card.transform.SetAsLastSibling();
            }
            else
            {
                card.transform.SetAsFirstSibling();
            }
            if (isInstant)
            {
                card.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                card.transform.localScale = Vector3.one;
                OnCardMovedIn?.Invoke();
            }
            else
                _moveQueue.Enqueue(card.transform as RectTransform);
            OnCardMovedIn?.Invoke();
        }

        internal void DestroyCards()
        {
            List<Card> cardsToDestroy = new(GetComponentsInChildren<Card>());
            foreach (var card in cardsToDestroy)
            {
                card.transform.DOKill(); // Stop any ongoing animations
                card.transform.SetParent(null); // Unparent the card before destroying
                card.transform.DOScale(Vector3.zero, 0.3f)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        card.gameObject.SetActive(false);
                        StartCoroutine(DelayedDestroyed(card));
                    });
            }
        }

        private IEnumerator DelayedDestroyed(Card card)
        {
            card.transform.DOKill();
            yield return null;
            yield return null;
            Destroy(card.gameObject);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnTouchPress?.Invoke(this);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            OnTouchRelease?.Invoke(this);
        }
    }
}
