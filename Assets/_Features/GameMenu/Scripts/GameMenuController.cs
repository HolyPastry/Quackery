using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Quackery.Artifacts;
using Quackery.Bills;
using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery.GameMenu
{
    public class GameMenuController : MonoBehaviour
    {

        [SerializeField] private Toggle _menuToggle;
        [SerializeField] private GameMenuPanel _menuPanel;
        [SerializeField] private CollectionUpdate _cardUpdate;
        [SerializeField] private CollectionUpdate _artifactUpdate;
        [SerializeField] private CollectionUpdate _billUpdate;

        [SerializeField] private PurseUpdate _purseUpdate;

        [SerializeField] private DestroyZone _destroyZone;

        [SerializeField] private RectTransform _drawPileUITransform;

        [SerializeField] private ArtifactUI _prefab;

        private bool _isMenuOpen;

        public static event Action<bool> OnGameMenuToggle = delegate { };

        public static Action ShowRequest = delegate { };
        public static Action HideRequest = delegate { };

        public static Func<List<Card>, Coroutine> AddToDeckRequest = (card) => null;
        public static Func<ItemData, bool, Coroutine> RemoveFromDeckRequest = (card, allMatchingCard) => null;

        public static Func<RectTransform, Coroutine> AddToArtifactRequest = (artifact) => null;

        public static Func<ArtifactData, Coroutine> RemoveFromArtifactRequest = (artifact) => null;

        public static Func<RectTransform, Coroutine> AddToBillsRequest = (bill) => null;
        public static Func<BillData, Coroutine> RemoveFromBillsRequest = (bill) => null;
        internal static Func<Coroutine> TakeArtifactsOut = () => null;
        internal static Func<Coroutine> TakeDeckOut = () => null;

        private bool _initialized;

        public static Action OnCardMovement = () => { };

        void Awake()
        {
            _menuToggle.isOn = false;
            TweenUtil.TeleportOut(_cardUpdate.transform as RectTransform);
            TweenUtil.TeleportOut(_artifactUpdate.transform as RectTransform);
            TweenUtil.TeleportOut(_billUpdate.transform as RectTransform);
            TweenUtil.TeleportOut(_purseUpdate.RectTransform);
            TweenUtil.TeleportOut(_menuToggle.transform as RectTransform);
        }

        void OnEnable()
        {
            _menuToggle.onValueChanged.AddListener(OnMenuToggleChanged);

            AddToDeckRequest = (cards) => StartCoroutine(AddToDeckRoutine(cards));
            RemoveFromDeckRequest = (card, allMatchingCards) => StartCoroutine(RemoveFromDeckRoutine(card, allMatchingCards));

            AddToArtifactRequest = (artifact) => StartCoroutine(AddToArtifactsRoutine(artifact));
            RemoveFromArtifactRequest = (artifact) => StartCoroutine(RemoveFromArtifactsRoutine(artifact));

            AddToBillsRequest = (bill) => StartCoroutine(AddToBillRoutine(bill));
            RemoveFromBillsRequest = (bill) => StartCoroutine(RemoveFromBillsRoutine(bill));

            TakeArtifactsOut = () => StartCoroutine(TakeArtifactsOutRoutine());
            TakeDeckOut = () => StartCoroutine(TakeDeckOutRoutine());


            if (_initialized)
                PurseEvents.OnPurseUpdated += UpdatePurse;

            ShowRequest += Show;
            HideRequest += Hide;

        }



        void OnDisable()
        {
            _menuToggle.onValueChanged.RemoveListener(OnMenuToggleChanged);

            AddToDeckRequest = (cards) => null;
            RemoveFromDeckRequest = (card, allMatchingCards) => null;

            AddToArtifactRequest = (artifact) => null;
            RemoveFromArtifactRequest = (artifact) => null;

            AddToBillsRequest = (bill) => null;
            RemoveFromBillsRequest = (bill) => null;

            TakeArtifactsOut = () => null;
            TakeDeckOut = () => null;

            PurseEvents.OnPurseUpdated -= UpdatePurse;

            ShowRequest = delegate { };
            HideRequest = delegate { };
        }

        IEnumerator Start()
        {
            yield return FlowServices.WaitUntilEndOfSetup();
            yield return PurseServices.WaitUntilReady();
            _purseUpdate.UpdateUI();
            _menuPanel.TeleportOffscreen();
            PurseEvents.OnPurseUpdated += UpdatePurse;
            _initialized = true;
        }

        private void UpdatePurse(float amount) => StartCoroutine(UpdatePurseRoutine(amount));


        private IEnumerator UpdatePurseRoutine(float amount)
        {

            _menuToggle.isOn = false;
            TweenUtil.SlideIn(_purseUpdate.RectTransform);
            yield return new WaitForSeconds(0.7f);
            yield return StartCoroutine(_purseUpdate.AddMoney((int)amount));
            yield return new WaitForSeconds(1f);
            TweenUtil.SlideOut(_purseUpdate.RectTransform);
            yield return new WaitForSeconds(0.5f);
        }



        private IEnumerator RemoveFromBillsRoutine(BillData billData)
        {
            _menuToggle.isOn = false;
            var block = _billUpdate.PutOut(billData);

            TweenUtil.SlideIn(_billUpdate.RectTransform);

            yield return new WaitForSeconds(0.5f);
            yield return StartCoroutine(_destroyZone.DestroyBill(block));

            TweenUtil.SlideOut(_billUpdate.RectTransform);
            yield return new WaitForSeconds(0.5f);
        }

        private IEnumerator AddToBillRoutine(RectTransform bill)
        {
            _menuToggle.isOn = false;

            TweenUtil.SlideIn(_billUpdate.RectTransform);
            StartCoroutine(_billUpdate.MoveIn(bill));
            yield return new WaitForSeconds(0.7f);
            TweenUtil.SlideOut(_billUpdate.RectTransform);
            yield return new WaitForSeconds(0.5f);
            _billUpdate.DestroyItems();
        }

        private IEnumerator RemoveFromArtifactsRoutine(ArtifactData artifactData)
        {
            _menuToggle.isOn = false;
            var artifactIcon = _artifactUpdate.PutOut(artifactData);

            TweenUtil.SlideIn(_artifactUpdate.RectTransform);

            yield return new WaitForSeconds(0.5f);
            yield return StartCoroutine(_destroyZone.DestroyArtifact(artifactIcon));

            TweenUtil.SlideOut(_artifactUpdate.RectTransform);
            yield return new WaitForSeconds(0.5f);
        }

        private IEnumerator AddToArtifactsRoutine(RectTransform artifactIcon)
        {
            _menuToggle.isOn = false;

            TweenUtil.SlideIn(_artifactUpdate.RectTransform);
            StartCoroutine(_artifactUpdate.MoveIn(artifactIcon));
            yield return new WaitForSeconds(0.7f);

            TweenUtil.SlideOut(_artifactUpdate.RectTransform);
            yield return new WaitForSeconds(0.5f);
            _artifactUpdate.DestroyItems();
        }

        private IEnumerator RemoveFromDeckRoutine(ItemData itemData, bool allMatching)
        {
            var items = InventoryServices.GetAllItems();
            items = items.FindAll(item => item.Data == itemData);
            if (items.Count == 0) yield break;
            _menuToggle.isOn = false;

            List<Card> newCards = new List<Card>();
            if (allMatching)
            {
                foreach (var item in items)
                {
                    var newCard = _cardUpdate.PutOut(item.Data);
                    newCards.Add(newCard);
                }
            }
            else
            {
                var newCard = _cardUpdate.PutOut(itemData);
                newCards.Add(newCard);
            }
            TweenUtil.SlideIn(_cardUpdate.RectTransform);
            yield return new WaitForSeconds(0.5f);
            foreach (var newCard in newCards)
            {
                OnCardMovement?.Invoke();
                yield return StartCoroutine(_destroyZone.DestroyCard(newCard));
            }
            TweenUtil.SlideOut(_cardUpdate.RectTransform);
            yield return new WaitForSeconds(0.5f);
        }

        private IEnumerator AddToDeckRoutine(List<Card> cards)
        {
            _menuToggle.isOn = false;


            TweenUtil.SlideIn(_cardUpdate.RectTransform);
            yield return new WaitForSeconds(0.5f);
            foreach (var card in cards)
            {
                StartCoroutine(_cardUpdate.MoveIn(card.RectTransform));
                OnCardMovement?.Invoke();
                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(1f);
            TweenUtil.SlideOut(_cardUpdate.RectTransform);
            yield return new WaitForSeconds(0.5f);
            _cardUpdate.DestroyItems();

        }






        private void OnMenuToggleChanged(bool isOn)
        {
            if (isOn == _isMenuOpen) return;
            _isMenuOpen = isOn;
            if (isOn)
                _menuPanel.SlideIn();
            else
                _menuPanel.SlideOut();

            OnGameMenuToggle.Invoke(isOn);
        }


        private void Hide()
        {
            _menuToggle.isOn = false;
            TweenUtil.SlideOut(_menuToggle.transform as RectTransform);
        }

        private void Show()
        {
            TweenUtil.SlideIn(_menuToggle.transform as RectTransform);
        }

        private IEnumerator TakeDeckOutRoutine()
        {
            if (_drawPileUITransform == null)
            {
                Debug.LogWarning("Draw Pile UI Transform is not set.");
                yield break;
            }
            var items = InventoryServices.GetAllItems();
            if (items.Count == 0) yield break;
            _menuToggle.isOn = false;

            List<Card> newCards = new();
            foreach (var item in items)
            {
                var newCard = _cardUpdate.PutOut(item.Data);
                newCards.Add(newCard);
            }

            TweenUtil.SlideIn(_cardUpdate.RectTransform);
            yield return new WaitForSeconds(0.5f);

            foreach (var newCard in newCards)
            {
                OnCardMovement?.Invoke();
                newCard.RectTransform.DOAnchorPos(new Vector2(300, 1000), 0.2f)
                    .SetEase(Ease.OutSine)
                    .OnComplete(
                        () => newCard.transform.SetParent(_drawPileUITransform, false));


                yield return null;

            }
            TweenUtil.SlideOut(_cardUpdate.RectTransform);
            yield return new WaitForSeconds(0.5f);
            for (int i = newCards.Count - 1; i >= 0; i--)
            {
                var newCard = newCards[i];
                OnCardMovement?.Invoke();
                newCard.RectTransform.DOAnchorPos(Vector2.zero, 0.2f).SetEase(Ease.OutSine);
                newCard.RectTransform.DOScale(Vector3.one, 0.2f)
                    .OnComplete(() => Destroy(newCard.gameObject));

                yield return null;
            }
        }

        private IEnumerator TakeArtifactsOutRoutine()
        {
            var artifacts = ArtifactServices.GetAllArtifacts();
            if (artifacts.Count == 0) yield break;
            _menuToggle.isOn = false;
            TweenUtil.SlideIn(_artifactUpdate.RectTransform);

            yield return new WaitForSeconds(0.5f);
            foreach (var artifact in artifacts)
            {
                var artifactIcon = Instantiate(_prefab, _artifactUpdate.Parent);
                artifactIcon.RectTransform.localScale = Vector3.one * 3f;
                artifactIcon.RectTransform.anchoredPosition = Vector2.zero;
                artifactIcon.Artifact = artifact;
                yield return new WaitForSeconds(0.2f);
                ArtifactBarUI.TransferRequest(artifactIcon);
                yield return new WaitForSeconds(0.5f);
            }
            TweenUtil.SlideOut(_artifactUpdate.RectTransform);

        }


    }
}
