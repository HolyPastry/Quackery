using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Quackery.Inventories;

using Quackery.Effects;
using System.Linq;
using UnityEngine.Assertions;
using DG.Tweening;
using System;



namespace Quackery.Decks
{


    public class DeckController : MonoBehaviour
    {

        [SerializeField] private int _initialHandSize = 4;

        [SerializeField] private EffectPileController _effectPileController;
        [SerializeField] private DrawPile _drawPile;

        [SerializeField] private CardPile _discardPile;
        [SerializeField] private CardPile _exhaustPile;

        [SerializeField] private CartController _cartController;
        [SerializeField] private HandController _handController;


        private int HandSize
        {
            get
            {
                int handSizeModifier = (int)EffectServices.GetModifier(typeof(HandSizeEffect));

                return _initialHandSize + handSizeModifier;
            }
        }

        #region Service Properties

        private bool _cardPlayed;
        private bool _isReady;
        private Card _cardBeingPlayed;
        private int _customDraw;
        private Card _cardSelected;
        private List<Card> _otherCards = new();
        private int _handSize;

        #endregion


        #region Service Hookups


        void OnDisable()
        {
            DeckServices.WaitUntilReady = () => new WaitUntil(() => false);

            //  DeckServices.AddNewToDiscard = (itemData, amount) => { };
            DeckServices.Shuffle = () => { };

            DeckServices.DrawBackToFull = () => null;

            DeckServices.DiscardHand = () => null;
            DeckServices.Discard = (card) => null;
            DeckServices.DiscardCards = (amount) => null;

            //  DeckServices.GetTablePile = () => new();
            DeckServices.SelectCard = (cardPile) => { };
            DeckServices.MovePileType = (sourcePile, targetPile) => { };
            DeckServices.GetTopCard = (pileType) => null;

            DeckServices.MoveToCardSelect = (cards) => { };
            DeckServices.MoveToHand = (card) => { };
            DeckServices.ActivateHand = () => { };
            DeckServices.DeactivateHand = () => { };

            DeckServices.ChangeCardCategory = delegate { };
            DeckServices.RestoreCardCategories = () => { };


            DeckServices.CardPlayed = () => false;
            DeckServices.GetCardPoolSize = (cardPileType) => 0;

            DeckServices.NoPlayableCards = () => false;

            // DeckServices.MoveToPile = (source, target) => { };
            DeckServices.StartPlayCardLoop = delegate { };
            DeckServices.StopPlayCardLoop = () => { };

            DeckServices.BoostPriceOfCardsInHand = (value, predicate) => { };
            // DeckServices.AddNew =
            //     (itemData, pileType, pileLocation, lifetime) => null;
            DeckServices.MoveCard = (card, pileType, placement, delay) => null;
            DeckServices.MoveToPile = (cards, targetPile) => null;


            DeckServices.GetMatchingCards = (condition, pile) => new List<Card>();
            DeckServices.ResetDecks = () => { };
            DeckServices.SetCustomDraw = (numCard) => { };

            DeckServices.IsPilePlayable = (type, index) => true;

            DeckServices.RemoveFromAllPiles = (card) => { };
            DeckServices.DestroyCardType = (itemData) => null;
            DeckServices.DestroyEffemeralCards = () => { };

            DeckServices.IsHandFull = () => false;
            DeckServices.IsHandEmpty = () => false;

            EffectEvents.OnAdded -= UpdateCardUI;
            EffectEvents.OnRemoved -= UpdateCardUI;
            EffectEvents.OnUpdated -= UpdateCardUI;
            Reset();

        }

        void OnEnable()
        {
            DeckServices.WaitUntilReady = () => new WaitUntil(() => _isReady);

            //DeckServices.AddNewToDiscard = AddNewToDiscard;
            DeckServices.Shuffle = ShuffleDiscardPileIn;

            DeckServices.DrawBackToFull = () => StartCoroutine(DrawBackToFull());

            DeckServices.Discard = (cards) => StartCoroutine(Discard(cards));
            DeckServices.DiscardHand = () => DeckServices.DiscardCards(-1);
            DeckServices.DiscardCards = (numCards) => StartCoroutine(DiscardRandomCards(numCards));

            DeckServices.RemoveFromAllPiles = RemoveFromAllPiles;

            DeckServices.SelectCard = SelectCard;
            //  DeckServices.GetTablePile = () => _handPiles;
            DeckServices.MovePileType = MovePileTo;
            DeckServices.GetTopCard = GetTopCard;

            DeckServices.MoveToCardSelect = MoveToCardSelectionPile;
            DeckServices.MoveToHand = AddCardToHand;
            DeckServices.ActivateHand = ActivateHand;
            DeckServices.DeactivateHand = DeactivateAllPiles;

            DeckServices.ChangeCardCategory = ChangeCardCategory;
            DeckServices.RestoreCardCategories = RestoreCardCategories;
            DeckServices.ResetDecks = ResetDecks;


            DeckServices.CardPlayed = () => _cardPlayed;
            DeckServices.GetCardPoolSize = GetCardPoolSize;

            DeckServices.NoPlayableCards = _handController.NoPlayableCards;


            DeckServices.StartPlayCardLoop = StartPlayCardLoop;
            DeckServices.StopPlayCardLoop = StopPlayCardLoop;

            DeckServices.BoostPriceOfCardsInHand = BoostPriceOfCardsInHand;
            DeckServices.AddNew = AddNew;
            DeckServices.MoveCard = (card, enumPile, EnumPlacement, delay) => StartCoroutine(DelayedMoveCard(card, enumPile, EnumPlacement, delay));
            DeckServices.MoveToPile = (cards, targetPile) => StartCoroutine(MoveToPileRoutine(cards, targetPile));

            DeckServices.GetMatchingCards = GetMatchingCards;
            DeckServices.SetCustomDraw = (numDraw) => _customDraw = Mathf.Max(numDraw, _customDraw);
            DeckServices.IsHandFull = () => _handController.OccupiedPiles.Count() >= HandSize;
            DeckServices.IsHandEmpty = () => _handController.OccupiedPiles.Count() == 0;


            //TODO: reconnect IS Pile Playable
            // DeckServices.IsPilePlayable = (type, index) => _handPiles.Any(p => p.Type == type &&
            //                                                                     p.Playable);
            DeckServices.DestroyCardType = (itemData) => StartCoroutine(DestroyCardType(itemData));
            DeckServices.DestroyEffemeralCards = DestroyEffemeralCards;
            EffectEvents.OnAdded += UpdateCardUI;
            EffectEvents.OnRemoved += UpdateCardUI;
            EffectEvents.OnUpdated += UpdateCardUI;
            Initialize();
        }



        public void Initialize()
        {
            _cardPlayed = false;

            _cardBeingPlayed = null;
            _cardSelected = null;
            _customDraw = 0;

            _otherCards.Clear();

            _drawPile.Populate();
            _isReady = true;
        }

        public void Reset()
        {
            _drawPile.Clear();
            _isReady = false;
        }


        private IEnumerable<Card> GetMatchingCards(System.Predicate<Card> predicate, EnumCardPile pile)
        {

            return pile switch
            {
                EnumCardPile.Draw => _drawPile.Cards.Where(c => predicate(c)),
                EnumCardPile.Discard => _discardPile.Cards.Where(c => predicate(c)),
                EnumCardPile.Exhaust => _exhaustPile.Cards.Where(c => predicate(c)),
                EnumCardPile.Hand => _handController.Cards.Where(c => predicate(c)),
                EnumCardPile.Cart => CartServices.GetMatchingCards(predicate),

                _ => null,
            };
        }

        private void MoveCard(Card card, EnumCardPile pile, EnumPlacement placement, float delay)
        {
            if (delay <= 0)
                MoveCard(card, pile, placement);
            else
                StartCoroutine(DelayedMoveCard(card, pile, placement, delay));
        }


        private IEnumerator DelayedMoveCard(Card card, EnumCardPile pile, EnumPlacement placement, float delay)
        {
            yield return new WaitForSeconds(delay);
            MoveCard(card, pile, placement);
        }
        private IEnumerator MoveToPileRoutine(IEnumerable<Card> cards, CardPile targetPile)
        {
            foreach (var card in cards.ToList())
            {
                RemoveFromAllPiles(card);
                targetPile.AddAtTheBottom(card, isInstant: false);
            }
            yield return Tempo.WaitForABeat;
        }


        private void MoveCard(Card card, EnumCardPile pile, EnumPlacement placement)
        {
            RemoveFromAllPiles(card);
            switch (pile)
            {
                case EnumCardPile.Draw:
                    _drawPile.Add(card, placement);
                    break;
                case EnumCardPile.Discard:
                    _discardPile.Add(card, placement);
                    break;
                case EnumCardPile.Exhaust:
                    _exhaustPile.Add(card, placement);
                    break;

                case EnumCardPile.Effect:
                    _effectPileController.Teleport(card);
                    // MoveToEffectPile(card, true);
                    break;
                default:
                    Debug.LogWarning("Cannot add card to: " + pile);
                    break;
            }
        }
        private Card AddNew(ItemData data,
                         EnumCardPile pile,
                          EnumPlacement placement,
                           EnumLifetime lifetime)
        {

            Card card = DeckServices.CreateCard(data);
            if (lifetime == EnumLifetime.Permanent)
                InventoryServices.AddItem(card.Item);

            card.Item.Lifetime = lifetime;
            card.transform.localScale = Vector3.zero;
            MoveCard(card, pile, placement);
            card.transform.DOScale(1, Tempo.EighthBeat);
            return card;
        }

        private void BoostPriceOfCardsInHand(int bonus, System.Predicate<Card> predicate)
        {
            // foreach (var pile in _handPiles)
            // {
            //     if (pile.IsEmpty || !pile.Enabled) continue;

            //     if (predicate(pile.TopCard))
            //     {
            //         pile.TopCard.InHandPriceBonus += bonus;
            //         pile.TopCard.UpdateUI();
            //     }
            // }
        }

        private IEnumerator DestroyCardType(ItemData data)
        {
            if (data == null) yield break;

            // var card = _cards.Where(c => c.Item.Data == data)
            //             .First();
            // if (card == null) yield break;
            // DeckServices.DestroyCard(card);

        }

        private void DestroyEffemeralCards()
        {
            // _cards.FindAll(c => c.Item.Lifetime == EnumLifetime.Effemeral)
            //      .ForEach(card => DeckServices.DestroyCard(card));
        }


        private void StartPlayCardLoop(Card card)
        {
            _cardBeingPlayed = card;
            CartServices.SetStacksHighlights(card);
        }
        private void StopPlayCardLoop()
        {

            var selectedPile = CartServices.GetHoveredPile();
            CartServices.SetStacksHighlights(null);
            if (selectedPile == null) return;


            DeactivateAllPiles();
            if (_cardBeingPlayed != null)
                StartCoroutine(PlayCardRoutine(_cardBeingPlayed, selectedPile));

        }

        private int GetCardPoolSize(EnumCardPile cardPileType)
        {
            return 0;
            // if (cardPileType == EnumCardPile.Cart)
            //     return CartServices.GetCartSize();
            // return _cardPiles.Count(p => p.Type == cardPileType && p.Enabled);
        }

        private Card GetTopCard(EnumCardPile type)
        {
            return null;
            // if (type == EnumCardPile.Draw)
            //     return _drawPile.TopCard;

            // if (type == EnumCardPile.Discard)
            //     return _discardPile.TopCard;

            // if (type == EnumCardPile.Cart)
            //     return CartServices.GetTopCard();

            // var pile = _cardPiles.Find(p => p.Type == type);
            // return pile?.TopCard;
        }





        #endregion

        #region Managing Effects 


        private void RestoreCardCategories()
        {
            // foreach (var pile in _handPiles)
            // {
            //     if (pile.IsEmpty || !pile.Enabled) continue;
            //     pile.RestoreCategory();
            // }
            // CartServices.RestoreCategories();

        }

        private void ChangeCardCategory(EnumItemCategory category, EnumCardSelection selection)
        {
            if (selection == EnumCardSelection.Random)
            {
                ChangeRandomCardCategoryOnTable(category);
                return;
            }
            if (selection == EnumCardSelection.AllStack)
            {
                CartServices.ChangeCardCategory(category);

                return;
            }

        }
        private void ChangeRandomCardCategoryOnTable(EnumItemCategory category)
        {
            // CardPile impactedPile = null;

            // foreach (var pile in _handPiles)
            // {
            //     if (pile.IsEmpty) continue;
            //     if (pile.TopCard.Category == category) continue;

            //     impactedPile = pile;
            //     break;
            // }
            // if (impactedPile == null) return;

            // var topCard = impactedPile.TopCard;
            // if (topCard.Category == category) return;
            // topCard.OverrideCategory(category);
        }


        private void UpdateCardUI(Effect _) => UpdateCardUI();


        private void UpdateCardUI()
        {
            _handController.EnabledPiles.ToList().ForEach(p => p.UpdateUI());

            // _selectPiles.ForEach(pile => pile.UpdateUI());

        }

        private IEnumerator UpdateHandSizeRoutine(int handSizeModifier)
        {
            if (!_isReady) yield break;
            int newHandSize = _initialHandSize + handSizeModifier;
            newHandSize = Mathf.Max(newHandSize, 0);

            int diff = newHandSize - _handSize;

            if (diff > 0)
            {

                for (int i = 0; i < diff; i++)
                {
                    // int maxIndex = _handPiles.Max(p => p.Index);
                    // _cardPiles.Add(new CardPileUI(EnumCardPile.Hand, maxIndex + 1));
                    DeckEvents.OnCardPoolSizeIncrease(EnumCardPile.Hand);
                    yield return Tempo.WaitForABeat;
                }

            }
            else
            {
                for (int i = 0; i < Mathf.Abs(diff); i++)
                {
                    // var emptyCardPiles = _handPiles.FindAll(p => p.IsEmpty && p.Enabled);
                    // CardPile pile;
                    // if (emptyCardPiles.Count > 0)
                    //     pile = emptyCardPiles[0];
                    // else
                    //     pile = _handPiles.OrderBy(p => p.Index).LastOrDefault();

                    // if (!pile.IsEmpty)
                    // {
                    //     yield return StartCoroutine(Discard(pile.TopCard));
                    // }
                    // _cardPiles.Remove(pile);
                    // DeckEvents.OnCardPoolSizeDecrease(EnumCardPile.Hand, pile.Index);
                    yield return Tempo.WaitForABeat;
                }
            }
            _handSize = newHandSize;
        }

        // private void UpdateHandSize()
        // {
        //     throw new NotImplementedException();
        // }


        #endregion




        #region Player Interations
        private void SelectCard(CardPile pile)
        {
            DeactivateAllPiles();
            if (pile.Type == EnumCardPile.Hand)
                SelectCardFromHand(pile);

            if (pile.Type == EnumCardPile.Selection)
                ClickOnCardSelection(pile);
        }

        private void ClickOnCardSelection(CardPile pile)
        {
            // var pile = _cardPiles.Find(p => p.Type == type && p.Index == index);
            Assert.IsTrue(pile != null && !pile.IsEmpty, "called pile is null or Empty");

            var otherCards = new List<Card>();

            // foreach (var selectPile in _selectPiles)
            // {
            //     if (selectPile.Index == index) continue; // Skip the pile that was clicked

            //     if (selectPile != null && !selectPile.IsEmpty)
            //         otherCards.Add(selectPile.TopCard);
            // }
            // DeckEvents.OnCardSelected(pile.TopCard, otherCards);
            // ActivateTableCards();

        }

        private void SelectCardFromHand(CardPile pile)
        {
            // var pile = _cardPiles.Find(p => p.Type == type && p.Index == index);

            Assert.IsNotNull(pile,
                $"called pile is null or Empty");
            if (pile.IsEmpty) return;

            StartCoroutine(PlayCardRoutine(pile.TopCard, null));
        }

        private IEnumerator PlayCardRoutine(Card card, CardPile selectedPile)
        {
            DeactivateAllPiles();
            DeckEvents.OnCardPlayed?.Invoke(card);

            RemoveFromAllPiles(card);


            if (card.Category == EnumItemCategory.Skills)
            {
                yield return _effectPileController.Move(card);
                yield return EffectServices.Execute(EnumEffectTrigger.OnCardPlayed, card);
                yield return StartCoroutine(Discard(card));

            }
            else if (AddToCart(card, selectedPile))
            {
                yield return EffectServices.Execute(EnumEffectTrigger.OnCardPlayed, card);
                if (selectedPile != null && (selectedPile.TopCard == card))
                {
                    yield return EffectServices.ExecutePile(EnumEffectTrigger.BeforeCartCalculation, selectedPile);
                    yield return Tempo.WaitForEighthBeat;
                }

                yield return CartServices.CalculateCart();

            }
            else
            {
                Debug.LogWarning("Could not do anything with: " + card.Name);
                yield return StartCoroutine(Discard(card));
            }




            _handController.DisableEmptyPiles();
            _cardPlayed = true;
            _cardBeingPlayed = null;
            // card.InHandPriceBonus = 0;
            // card.UpdateUI();
            // UpdateCardUI();
        }

        private bool AddToCart(Card card, CardPile selectedPile)
        {
            return (selectedPile == null && CartServices.AddCardToCart(card)) ||
                 (selectedPile != null && CartServices.AddCardToCartPile(card, selectedPile));
        }

        #endregion


        #region Discarding Cards
        private IEnumerator DiscardRandomCards(int numCard)
        {
            if (numCard < 0)
            {
                yield return StartCoroutine(DiscardHand());
                yield break;
            }
            List<int> tablePileIndexes = new();
            // for (int i = 0; i < _handPiles.Count; i++)
            // {
            //     if (_handPiles[i].IsEmpty || !_handPiles[i].Enabled) continue;
            //     tablePileIndexes.Add(i);
            // }
            tablePileIndexes.Shuffle();

            int numCardToDiscard = Mathf.Min(numCard, tablePileIndexes.Count);
            if (numCardToDiscard <= 0) yield break;
            for (int i = 0; i < numCardToDiscard; i++)
            {
                // var pile = _handPiles[tablePileIndexes[i]];
                // yield return StartCoroutine(Discard(pile.TopCard));
            }
        }
        private IEnumerator Discard(List<Card> cards)
        {
            foreach (var card in cards)
            {
                yield return StartCoroutine(Discard(card));
            }
        }

        private IEnumerator Discard(Card card)
        {
            if (card == null)
            {
                Debug.LogWarning("Tried to discard a null card.", this);
                yield break;
            }
            EffectServices.RemoveLinkedToObject(card);
            if (_exhaustPile.Contains(card)) yield break;

            RemoveFromAllPiles(card);
            _discardPile.AddOnTop(card);
            yield return new WaitForSeconds(Tempo.EighthBeat);
            card.InHandPriceBonus = 0;
            card.UpdateUI();
            card.Discard();
        }

        private IEnumerator DiscardHand()
        {
            foreach (var card in _handController.TopCards)
            {
                yield return EffectServices.Execute(EnumEffectTrigger.OnDiscard, card);
                yield return StartCoroutine(Discard(card));
            }
        }


        #endregion

        #region Card Pile Manipulation

        private void RemoveFromAllPiles(Card card)
        {
            _drawPile.RemoveCard(card);


            _exhaustPile.RemoveCard(card);
            //    DeckEvents.OnPileUpdated(_exhaustPile.Type, _exhaustPile.Index);

            _discardPile.RemoveCard(card);
            //  DeckEvents.OnPileUpdated(_discardPile.Type, _discardPile.Index);

            _handController.RemoveCard(card);
            // foreach (var pile in _handPiles)
            // {

            //     if (pile.RemoveCard(card))
            //         DeckEvents.OnPileUpdated(pile.Type, pile.Index);
            // }

            CartServices.RemoveCard(card, false);

            _effectPileController.RemoveCard(card);


            // foreach (var pile in _selectPiles)
            // {
            //     if (pile.RemoveCard(card))
            //         DeckEvents.OnPileUpdated(pile.Type, pile.Index);
            // }

        }

        private void ShuffleDiscardPileIn()
        {
            // SetPileActivation(_handPiles, false);
            _drawPile.MergeBelow(_discardPile);
            _drawPile.Shuffle();
            //DeckEvents.OnShuffle(EnumCardPile.Draw, _drawPile.Index, _drawPile._cards);
        }



        private void ResetDecks()
        {
            DeckServices.DestroyEffemeralCards();
            // foreach (var pile in _handPiles)
            // {
            //     if (pile.IsEmpty) continue;
            //     _drawPile.MergeBelow(pile);
            // }

            _drawPile.MergeBelow(_exhaustPile);
            _drawPile.MergeBelow(_discardPile);

            _drawPile.Shuffle();
            //  DeckEvents.OnShuffle(EnumCardPile.Draw, _drawPile.Index, _drawPile._cards);
        }

        private void AddCardToHand(Card card)
        {
            RemoveFromAllPiles(card);

            if (!_handController.AddToEmptyPile(card, increaseIfFull: true))
                StartCoroutine(Discard(card));
        }

        private void ActivateHand()
        {
            _cardPlayed = false;
            foreach (var pile in _handController.EnabledPiles)
            {
                if (pile.IsEmpty) continue;
                var topCard = pile.TopCard;
                pile.Playable = false;
                if (topCard.Category == EnumItemCategory.Skills)
                {
                    if (topCard.Price > 0) pile.Playable = true;
                    else
                        pile.Playable = CartServices.CanCartAfford(-topCard.Price);
                }
                else if (topCard.Category == EnumItemCategory.Curse ||
                         topCard.Category == EnumItemCategory.TempCurse)
                {
                    pile.Playable = false;
                }
                else
                {
                    pile.Playable = CartServices.CanAddToCart(topCard);
                }

                pile.Playable &= CardEffectServices.IsPlayable(topCard);
                pile.SetActivated(pile.Playable);
            }


        }


        private void MoveToCardSelectionPile(List<Card> list)
        {
            DeckEvents.OnCardsMovingToSelectPile();
            foreach (var card in list)
            {
                // foreach (var pile in _selectPiles)
                // {
                //     if (pile.IsEmpty)
                //     {
                //         pile.AddOnTop(card);

                //         break;
                //     }
                // }
            }

            //SetPileActivation(_selectPiles, false);
        }



        private void MovePileTo(EnumCardPile pileType1, EnumCardPile pileType2)
        {
            // var pile1 = _cardPiles.Find(p => p.Type == pileType1);
            // var pile2 = _cardPiles.Find(p => p.Type == pileType2);

            // MovePileTo(pile1, pile2);

        }
        public static void MovePileTo(CardPile pile1, CardPile pile2)
        {
            if (pile1 == null || pile2 == null || pile1.IsEmpty) return;

            pile2.MergeBelow(pile1);
            // DeckEvents.OnPileUpdated(pile1.Type, pile1.Index);
            // DeckEvents.OnPileUpdated(pile2.Type, pile2.Index);
        }

        internal IEnumerator DrawBackToFull()
        {

            List<Card> drawnCards = new();

            // yield return StartCoroutine(CheckHandSize());

            if (_customDraw > 0)
            {
                drawnCards = _drawPile.DrawMany(_customDraw); ;
                DeckServices.MoveToCardSelect(drawnCards);
                DeckEvents.OnCardSelected += OnCardSelected;

                yield return new WaitUntil(() => _cardSelected != null);

                DeckEvents.OnCardSelected -= OnCardSelected;
                yield return EffectServices.Add(_cardSelected);
                yield return EffectServices.Execute(EnumEffectTrigger.OnDraw, _cardSelected);
                // yield return StartCoroutine(CheckHandSize());

                DeckServices.MoveToHand(_cardSelected);
                DeckServices.Discard(_otherCards);
                _customDraw = -1; // Reset custom draw after use
                _cardSelected = null;
                _otherCards = null;
            }

            int cardsNeeded = Mathf.Max(0, HandSize - _handController.OccupiedPiles.Count());
            int iteration = 100;
            do
            {
                drawnCards = _drawPile.DrawMany(cardsNeeded);
                foreach (var card in drawnCards)
                {
                    DeckServices.MoveToHand(card);
                    yield return EffectServices.Add(card);
                    yield return EffectServices.Execute(EnumEffectTrigger.OnDraw, card);
                    if (card == null) continue;
                    card.UpdateUI();
                }
                iteration--;

                cardsNeeded = Mathf.Max(0, HandSize - _handController.OccupiedPiles.Count());
            }
            while (cardsNeeded > 0 && EnoughCards(cardsNeeded) && iteration >= 0);

            yield return Tempo.WaitForABeat;
        }



        private bool EnoughCards(int cardsNeeded)
        {
            return _drawPile.Count + _discardPile.Count > cardsNeeded;
        }

        private void OnCardSelected(Card card, List<Card> list)
        {
            _cardSelected = card;
            _otherCards = list;
        }

        private void SetPileActivation(List<CardPile> piles, bool activated)
        {
            foreach (var pile in piles)
            {
                //DeckEvents.OnActivatePile(pile.Type, pile.Index, activated);
            }
        }

        private void DeactivateAllPiles()
        {
            _handController.OccupiedPiles.ToList().ForEach(p => p.SetActivated(false));
            //SetPileActivation(_handPiles, false);
            //SetPileActivation(_selectPiles, false);
        }
        #endregion
    }
}
