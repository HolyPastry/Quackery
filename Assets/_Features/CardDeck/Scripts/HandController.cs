using System;
using System.Collections.Generic;
using System.Linq;
using Quackery.Decks;
using TMPro;

using UnityEngine;


namespace Quackery
{
    public class HandController : CardPilePool
    {

        [Header("Hand Settings")]

        [SerializeField] private TextMeshProUGUI _helperText;
        [SerializeField] private float _cardHandRadius = 100f;
        [SerializeField] private float _sideMargin = 200f;

        [SerializeField] private RectTransform Aobj;
        [SerializeField] private RectTransform Bobj;
        [SerializeField] private RectTransform Cobj;
        [SerializeField] private RectTransform Dobj;


        [Header("CardControls")]
        [SerializeField] private float _maxSlideDistance = 420f;
        [SerializeField] private float _maxSlideDistanceY = 300f;
        [SerializeField] private float _selectedCardOffset = 150f;
        [SerializeField] private float _slideStartThreshold = 10f;

        [SerializeField] private float _maxAngleOffset = 10f;


        private const float NightyDegree = 90f;

        private CardPile _selectedPileUI;
        private CardPile _followTouchPointPileUI;
        private Vector2 _originalPosition;


        public IEnumerable<Card> Cards => _cardPiles.SelectMany(pile => pile.Cards);



        public static event Action<CardPile> OnCardSelected = delegate { };

        protected void OnEnable()
        {
            DeckEvents.OnCardPlayed += ResetSelectedPile;
            _helperText.text = "";
        }

        protected void OnDisable()
        {

            DeckEvents.OnCardPlayed -= ResetSelectedPile;
        }

        private void ResetSelectedPile(Card card)
        {
            _selectedPileUI = null;
            _followTouchPointPileUI = null;
            OnCardSelected?.Invoke(null);
            _helperText.text = "";
        }


        protected void SelectPile(CardPile pileUI)
        {
            if (pileUI.IsEmpty) return;
            foreach (var cardPile in _cardPiles)
                cardPile.transform.SetAsLastSibling();
            pileUI.transform.SetAsLastSibling();
            _selectedPileUI = pileUI;

            if (!DeckServices.IsPilePlayable(pileUI.Type, pileUI.PileIndex))
                _helperText.text = "No Playable Options";
            else if (pileUI.TopCard.HasCartTarget)
                _helperText.text = "Drag onto cart stack to play";
            else
            {
                if (pileUI.TopCard.Category == Inventories.EnumItemCategory.Skills)
                    _helperText.text = "Slide up to play";
                if (pileUI.TopCard.Category == Inventories.EnumItemCategory.Curse ||
                    pileUI.TopCard.Category == Inventories.EnumItemCategory.TempCurse)
                    _helperText.text = "Unplayable";
            }

            Tooltips.ShowTooltipRequest?.Invoke(pileUI.TopCard);
            DeckServices.StartPlayCardLoop(pileUI.TopCard);
            OnCardSelected?.Invoke(pileUI);
        }

        protected void DestroyCardPile(int index)
        {
            if (_selectedPileUI != null && _selectedPileUI.PileIndex == index)
                _selectedPileUI = null;
            if (_followTouchPointPileUI != null && _followTouchPointPileUI.PileIndex == index)
                _followTouchPointPileUI = null;


        }

        protected override void OnCardPileTouchPress(CardPile pileUI)
        {
            _helperText.text = "";
            _followTouchPointPileUI = pileUI;
            DeckServices.StartPlayCardLoop(pileUI.TopCard);

        }


        protected override void OnCardPileTouchRelease(CardPile ui)
        {
            _helperText.text = "";
            if (_followTouchPointPileUI == null)
                return;

            Tooltips.HideTooltipRequest();
            _followTouchPointPileUI = null;

            bool slid = (_originalPosition - Anchor.Instance.GetLocalMousePosition(ui.transform.parent as RectTransform))
                        .sqrMagnitude > _slideStartThreshold * _slideStartThreshold;

            if (slid)
            {

                DottedLine.HideDottedLine();

                if (CartServices.GetHoveredPile() == null)
                {
                    SelectPile(ui);
                    return;
                }
                else
                {
                    DeckServices.StopPlayCardLoop();
                }
            }
            else
            {
                if (_selectedPileUI == ui)
                {

                    _selectedPileUI = null;

                    OnCardSelected?.Invoke(null);

                    DeckServices.StopPlayCardLoop();
                }
                else
                {
                    SelectPile(ui);
                }
            }
        }

        void Update()
        {
            var center = (transform as RectTransform).anchoredPosition;


            Vector2 B = center;
            Vector2 A = B - 0.5f * (Screen.width - _sideMargin) * Vector2.right;
            Vector2 C = B + 0.5f * (Screen.width - _sideMargin) * Vector2.right;
            Vector2 D = B + _cardHandRadius * Vector2.down;


            Aobj.anchoredPosition = A;
            Bobj.anchoredPosition = B;
            Cobj.anchoredPosition = C;
            Dobj.anchoredPosition = D;

            var angle = Vector2.Angle(D - B, D - C);

            var cardPiles = EnabledPiles.ToList();

            int numCards = cardPiles.Count;

            if (numCards == 2) angle /= 3;
            if (numCards == 3) angle /= 1.5f;

            for (int i = 0; i < numCards; i++)
            {

                var cardPile = cardPiles[i];
                float t, angleOffset = 0f;

                if (numCards == 1)
                    t = 0.5f;
                else
                    t = (float)i / (numCards - 1);



                angleOffset = Mathf.Lerp(-angle, angle, t);
                float selectedOffset = 0f;
                float scale = 1f;
                if (cardPile == _selectedPileUI)
                {
                    selectedOffset = _selectedCardOffset;
                    //scale = 1.2f;
                    SetTopCardStraight(angleOffset, cardPile);
                }
                else
                {
                    ResetTopCardRotation(cardPile);
                }

                Vector2 position = D +
                        _cardHandRadius *
                        new Vector2(
                            Mathf.Cos((angleOffset + NightyDegree) * Mathf.Deg2Rad),
                             Mathf.Sin((angleOffset + NightyDegree) * Mathf.Deg2Rad));

                if (cardPile == _followTouchPointPileUI)
                {
                    _originalPosition = position;
                    FollowTouch(cardPile);
                    continue;
                }
                position += Vector2.up * selectedOffset;

                cardPile.AnchoredPosition = position;
                cardPile.transform.eulerAngles = new Vector3(0, 0, angleOffset);
                cardPile.transform.localScale = Vector3.one * scale;
            }

        }



        private void ResetTopCardRotation(CardPile cardPile)
        {
            if (cardPile.IsEmpty) return;
            var topCard = cardPile.TopCard;
            topCard.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

        private void SetTopCardStraight(float angleOffset, CardPile cardPile)
        {
            if (cardPile.IsEmpty) return;
            var topCard = cardPile.TopCard;
            topCard.transform.localRotation = Quaternion.Euler(0, 0, -angleOffset);
        }

        private void FollowTouch(CardPile cardPile)
        {

            var mousePosition = Anchor.Instance.GetLocalMousePosition(cardPile.transform.parent as RectTransform);


            var distanceFromOrigin = mousePosition - _originalPosition;
            //Debug.Log(distanceFromOrigin.sqrMagnitude);
            if (cardPile.HasCartTarget)
                distanceFromOrigin = Vector2.ClampMagnitude(distanceFromOrigin, _maxSlideDistance);
            else
                distanceFromOrigin = Vector2.ClampMagnitude(distanceFromOrigin, _maxSlideDistanceY);

            if (distanceFromOrigin.sqrMagnitude < _slideStartThreshold * _slideStartThreshold)
            {
                cardPile.AnchoredPosition = _originalPosition;
                return;
            }

            var targetPosition = _originalPosition + distanceFromOrigin;
            cardPile.AnchoredPosition = targetPosition;

            if (cardPile.HasCartTarget)
            {
                if (distanceFromOrigin.y > 0)
                    DottedLine.GenerateDottedLine(
                        cardPile.transform.position);
                else
                    DottedLine.HideDottedLine();
            }
            else
            {
                if (distanceFromOrigin.y > 0 &&
                    distanceFromOrigin.sqrMagnitude > _maxSlideDistanceY * _maxSlideDistanceY)
                {
                    DeckServices.SelectCard(cardPile);
                    _followTouchPointPileUI = null;
                    Tooltips.HideTooltipRequest?.Invoke();
                }
            }

            //cardPile.transform.position = targetPosition;
        }

        internal bool NoPlayableCards()
        {
            return !_cardPiles.Exists(p => !p.IsEmpty && p.Enabled && p.Playable);
        }


    }
}
