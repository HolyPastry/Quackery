using System;
using Quackery.Decks;
using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Quackery
{
    public class HandController : CardPool
    {

        [Header("Hand Settings")]

        [SerializeField] private TextMeshProUGUI _helperText;
        [SerializeField] private float _cardHandRadius = 100f;
        [SerializeField] private float _sideMargin = 200f;

        [SerializeField] private GameObject Aobj;
        [SerializeField] private GameObject Bobj;
        [SerializeField] private GameObject Cobj;
        [SerializeField] private GameObject Dobj;


        [Header("CardControls")]
        [SerializeField] private InputActionReference _mousePositionAction;
        [SerializeField] private float _maxSlideDistance = 420f;
        [SerializeField] private float _maxSlideDistanceY = 300f;
        [SerializeField] private float _selectedCardOffset = 150f;
        [SerializeField] private bool _useDottedLine = true;
        [SerializeField] private float _slideStartThreshold = 10f;


        private const float NightyDegree = 90f;

        private CardPileUI _selectedPileUI;
        private CardPileUI _followTouchPointPileUI;
        private Vector2 _originalPosition;

        public static event Action<CardPileUI> OnCardSelected = delegate { };

        protected override void OnEnable()
        {
            base.OnEnable();
            DeckEvents.OnCardPlayed += ResetSelectedPile;
            _helperText.text = "";
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            DeckEvents.OnCardPlayed -= ResetSelectedPile;
        }

        private void ResetSelectedPile(Card card)
        {
            _selectedPileUI = null;
            _followTouchPointPileUI = null;
            OnCardSelected?.Invoke(null);
            _helperText.text = "";
        }


        protected void SelectPile(CardPileUI pileUI)
        {
            if (pileUI.IsEmpty) return;
            foreach (var cardPile in _cardPileUIs)
                cardPile.transform.SetAsLastSibling();
            pileUI.transform.SetAsLastSibling();
            _selectedPileUI = pileUI;

            if (!DeckServices.IsPilePlayable(pileUI.Type, pileUI.PileIndex))
                _helperText.text = "No Playable Options";
            else if (pileUI.TopCard.HasCartTarget)
                _helperText.text = "Drag onto cart stack to play";
            else
            {
                if (pileUI.TopCard.Category == Inventories.EnumItemCategory.Skill)
                    _helperText.text = "Slide up to play";
                if (pileUI.TopCard.Category == Inventories.EnumItemCategory.Curse ||
                    pileUI.TopCard.Category == Inventories.EnumItemCategory.TempCurse)
                    _helperText.text = "Unplayable";
            }


            DeckServices.StartPlayCardLoop(pileUI.TopCard);
            OnCardSelected?.Invoke(pileUI);
        }

        protected override void DestroyCardPile(int index)
        {
            if (_selectedPileUI != null && _selectedPileUI.PileIndex == index)
                _selectedPileUI = null;
            if (_followTouchPointPileUI != null && _followTouchPointPileUI.PileIndex == index)
                _followTouchPointPileUI = null;
            base.DestroyCardPile(index);

        }

        protected override void OnCardPileTouchPress(CardPileUI pileUI)
        {
            _helperText.text = "";
            _followTouchPointPileUI = pileUI;
            DeckServices.StartPlayCardLoop(pileUI.TopCard);
            if (!pileUI.IsEmpty)
                Tooltips.ShowTooltipRequest?.Invoke(pileUI.TopCard);
        }


        protected override void OnCardPileTouchRelease(CardPileUI ui)
        {
            _helperText.text = "";
            if (_followTouchPointPileUI == null)
                return;

            _followTouchPointPileUI = null;

            bool slid = (_originalPosition - _mousePositionAction.action.ReadValue<Vector2>())
                        .sqrMagnitude > _slideStartThreshold * _slideStartThreshold;

            if (slid)
            {

                OverlayCanvas.HideDottedLine();

                if (CartServices.GetHoveredPile() == null)
                {
                    SelectPile(ui);
                    return;
                }
                else
                {

                    Tooltips.HideTooltipRequest();
                    DeckServices.StopPlayCardLoop();
                }
            }
            else
            {
                if (_selectedPileUI == ui)
                {
                    _selectedPileUI = null;
                    OnCardSelected?.Invoke(null);
                    Tooltips.HideTooltipRequest();
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
            Vector2 B = (Vector2)transform.position;
            Vector2 A = B - 0.5f * (Screen.width - _sideMargin) * Vector2.right;
            Vector2 C = B + 0.5f * (Screen.width - _sideMargin) * Vector2.right;
            Vector2 D = B + _cardHandRadius * Vector2.down;


            Aobj.transform.position = A;
            Bobj.transform.position = B;
            Cobj.transform.position = C;
            Dobj.transform.position = D;

            var angle = Vector2.Angle(D - B, D - C);

            for (int i = 0; i < _cardPileUIs.Count; i++)
            {

                var cardPile = _cardPileUIs[i];
                float t, angleOffset = 0f;
                if (_cardPileUIs.Count == 1)
                {
                    angleOffset = Mathf.Lerp(-angle, angle, 0.5f);
                }
                else
                {
                    t = (float)i / (_cardPileUIs.Count - 1);
                    angleOffset = Mathf.Lerp(-angle, angle, t);
                }
                float selectedOffset = 0f;
                float scale = 1f;
                if (cardPile == _selectedPileUI)
                {
                    selectedOffset = _selectedCardOffset;
                    scale = 1.2f;
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

                cardPile.transform.SetPositionAndRotation(position,
                             Quaternion.Euler(0, 0, angleOffset));
                cardPile.transform.localScale = Vector3.one * scale;
            }

        }

        private void ResetTopCardRotation(CardPileUI cardPile)
        {
            if (cardPile.IsEmpty) return;
            var topCard = cardPile.TopCard;
            topCard.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

        private void SetTopCardStraight(float angleOffset, CardPileUI cardPile)
        {
            if (cardPile.IsEmpty) return;
            var topCard = cardPile.TopCard;
            topCard.transform.localRotation = Quaternion.Euler(0, 0, -angleOffset);
        }

        private void FollowTouch(CardPileUI cardPile)
        {

            var mousePosition = _mousePositionAction.action.ReadValue<Vector2>();

            var distanceFromOrigin = mousePosition - _originalPosition;
            distanceFromOrigin = Vector2.ClampMagnitude(distanceFromOrigin, _maxSlideDistance);

            if (distanceFromOrigin.sqrMagnitude < _slideStartThreshold * _slideStartThreshold)
            {
                cardPile.transform.position = _originalPosition;
                return;
            }

            var targetPosition = _originalPosition + distanceFromOrigin;

            if (_useDottedLine && cardPile.HasCartTarget)
            {
                if (distanceFromOrigin.y > _maxSlideDistanceY)
                    OverlayCanvas.GenerateDottedLine(targetPosition, mousePosition);
                else
                    OverlayCanvas.HideDottedLine();
            }
            else
            {
                if (distanceFromOrigin.y > _maxSlideDistanceY)
                {
                    DeckServices.SelectCard(cardPile.Type, cardPile.PileIndex);
                    _followTouchPointPileUI = null;
                }
            }
            cardPile.transform.position = targetPosition;
        }
    }
}
