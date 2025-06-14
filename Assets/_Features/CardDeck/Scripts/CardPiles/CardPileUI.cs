
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Quackery.Decks
{
    public class CardPileUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] EnumPileType _pileType;
        [SerializeField] private float _moveSpeed = 0.5f;
        [SerializeField] private Ease _easeType = Ease.OutBack;
        [SerializeField] private float _staggerDelay = 0.1f;
        [SerializeField] private RewardPanel _rewardPanel;

        public EnumPileType Type => _pileType;

        public bool IsEmpty => transform.childCount == 0;

        public List<Card> Cards => new(GetComponentsInChildren<Card>());

        public object StackSize => GetComponentsInChildren<Card>().Length;

        // public event System.Action<CardPileUI> OnClicked;
        private readonly Queue<RectTransform> _moveQueue = new();

        void OnEnable()
        {
            DeckEvents.OnCardMovedTo += OnCardMoved;
            DeckEvents.OnShuffle += OnShuffle;
            DeckEvents.OnPileDestroyed += OnPileDestroyed;


            StartCoroutine(StaggeredMoveRoutine());

        }
        void OnDisable()
        {
            DeckEvents.OnCardMovedTo -= OnCardMoved;
            DeckEvents.OnShuffle -= OnShuffle;
            DeckEvents.OnPileDestroyed -= OnPileDestroyed;

            StopAllCoroutines(); // Stop all coroutines when disabled
        }


        private void OnPileDestroyed(EnumPileType type)
        {
            if (type != _pileType) return;

            DestroyCards();

        }

        private void OnShuffle(EnumPileType type, List<Card> cards)
        {
            if (type != _pileType) return;


            StartCoroutine(ShuffleCardRoutine(cards));
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
            while (true)
            {
                if (_moveQueue.Count > 0)
                {
                    var cardTransform = _moveQueue.Dequeue();
                    if (cardTransform == null)
                        continue; // Skip if the card transform is null

                    cardTransform.DOAnchorPos(Vector3.zero, _moveSpeed).SetEase(_easeType);
                    yield return new WaitForSeconds(_staggerDelay); // Stagger the movement of cards
                }
                else
                {
                    yield return null; // Wait for the next frame if no cards to move
                }
            }
        }

        private void OnCardMoved(Card card, EnumPileType type, bool atTheTop)
        {
            if (type != _pileType) return;

            MoveCardToPile(card, atTheTop);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            //noop
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            TooltipManager.ShowTooltipRequest(gameObject);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TooltipManager.HideTooltipRequest();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            DeckServices.PileClicked(_pileType);
        }

        internal void MoveCardToPile(Card card, bool atTheTop)
        {
            card.transform.SetParent(transform);
            card.transform.localScale = Vector3.one;
            if (atTheTop)
            {
                card.transform.SetAsLastSibling();

            }
            else
            {
                card.transform.SetAsFirstSibling();
            }
            _moveQueue.Enqueue(card.transform as RectTransform);
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

        internal void ShowReward(CardReward cardReward)
        {
            if (_rewardPanel == null)
            {
                Debug.LogWarning("RewardPanel is not assigned in CardPileUI.");
                return;
            }
            _rewardPanel.ShowReward(cardReward);
            _rewardPanel.transform.SetAsLastSibling();
        }
    }
}
