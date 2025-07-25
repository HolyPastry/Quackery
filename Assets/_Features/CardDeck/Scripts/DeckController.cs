using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Quackery.Inventories;
using System.Linq;
using Quackery.Effects;

using UnityEngine.Assertions;
using System;
using DG.Tweening;
using Quackery.Clients;


namespace Quackery.Decks
{


    public class DeckController : MonoBehaviour
    {
        [SerializeField] private Card _itemCardPrefab;
        [SerializeField] private Card _skillCardPrefab;
        [SerializeField] private Card _curseCardPrefab;


        [SerializeField] private int _initialHandSize = 4;
        [SerializeField] private int _initialSelectionSize = 4;

        private CardFactory _cardFactory;

        #region Service Properties

        private readonly List<CardPile> _cardPiles = new();




        private bool _cardPlayed;

        private DrawPile _drawPile;

        private bool _isReady;
        private Card _cardBeingPlayed;
        private int _customDraw;
        private Card _cardSelected;
        private List<Card> _otherCards;

        private CardPile _discardPile => _cardPiles.Find(p => p.Type == EnumCardPile.Discard);
        private List<CardPile> _handPiles => _cardPiles.FindAll(p => p.Type == EnumCardPile.Hand);
        private List<CardPile> _selectPiles => _cardPiles.FindAll(p => p.Type == EnumCardPile.Selection);

        private CardPile _exhaustPile => _cardPiles.Find(p => p.Type == EnumCardPile.Exhaust);

        #endregion


        #region Service Hookups

        void Awake()
        {
            _cardFactory = new CardFactory(_itemCardPrefab, _skillCardPrefab, _curseCardPrefab);
        }
        void OnDisable()
        {
            DeckServices.WaitUntilReady = () => new WaitUntil(() => true);

            //  DeckServices.AddNewToDiscard = (itemData, amount) => { };
            DeckServices.Shuffle = () => { };

            DeckServices.DrawBackToFull = () => null;

            DeckServices.DiscardHand = () => null;
            DeckServices.Discard = (card) => { };
            DeckServices.DiscardCards = (amount) => { };

            DeckServices.DestroyCard = (cards) => { };
            DeckServices.DuplicateCard = (card) => null;

            DeckServices.GetTablePile = () => new();
            DeckServices.SelectCard = (pileType, index) => { };
            DeckServices.MovePileType = (sourcePile, targetPile) => { };
            DeckServices.GetTopCard = (pileType) => null;

            DeckServices.MoveToCardSelect = (cards) => { };
            DeckServices.MoveToTable = (card) => { };
            DeckServices.ActivateTableCards = () => { };

            DeckServices.ChangeCardCategory = delegate { };
            DeckServices.RestoreCardCategories = () => { };


            DeckServices.CardPlayed = () => false;
            DeckServices.GetCardPoolSize = (cardPileType) => 0;

            DeckServices.NoPlayableCards = () => false;

            DeckServices.MoveToPile = (source, target) => { };

            DeckServices.CreateCard = (itemData) => null;


            DeckServices.StartPlayCardLoop = delegate { };
            DeckServices.StopPlayCardLoop = () => { };

            DeckServices.BoostPriceOfCardsInHand = (value, predicate) => { };
            DeckServices.AddNew =
                (itemData, pileType, pileLocation, lifetime) => null;
            DeckServices.MoveCard = (card, pileType, placement, delay) => { };

            DeckServices.ReplaceCard = (card, replacementCard) => { };
            DeckServices.GetMatchingCards = (condition, pile) => new List<Card>();
            DeckServices.ResetDecks = () => { };
            DeckServices.SetCustomDraw = (numCard) => { };

            EffectEvents.OnAdded -= UpdateCardUI;
            EffectEvents.OnRemoved -= UpdateCardUI;
            EffectEvents.OnUpdated -= UpdateCardUI;

        }

        void OnEnable()
        {
            DeckServices.WaitUntilReady = () => new WaitUntil(() => _isReady);

            //DeckServices.AddNewToDiscard = AddNewToDiscard;
            DeckServices.Shuffle = ShuffleDiscardPileIn;

            DeckServices.DrawBackToFull = DrawBackToFull;

            DeckServices.Discard = Discard;
            DeckServices.DiscardHand = DiscardHand;
            DeckServices.DiscardCards = DiscardRandomCards;

            DeckServices.DestroyCard = DestroyCard;
            DeckServices.DuplicateCard = DuplicateCard;

            DeckServices.SelectCard = SelectCard;
            DeckServices.GetTablePile = () => _handPiles;
            DeckServices.MovePileType = MovePileTo;
            DeckServices.GetTopCard = GetTopCard;

            DeckServices.MoveToCardSelect = MoveToCardSelectionPile;
            DeckServices.MoveToTable = AddCardToTable;
            DeckServices.ActivateTableCards = ActivateTableCards;

            DeckServices.ChangeCardCategory = ChangeCardCategory;
            DeckServices.RestoreCardCategories = RestoreCardCategories;
            DeckServices.ResetDecks = ResetDecks;


            DeckServices.CardPlayed = () => _cardPlayed;
            DeckServices.GetCardPoolSize = GetCardPoolSize;

            DeckServices.NoPlayableCards =
                    () => _handPiles.All(p => p.Enabled && (p.IsEmpty || !p.Playable));

            DeckServices.MoveToPile = MovePileTo;
            DeckServices.CreateCard = CreateCard;

            DeckServices.StartPlayCardLoop = StartPlayCardLoop;
            DeckServices.StopPlayCardLoop = StopPlayCardLoop;

            DeckServices.BoostPriceOfCardsInHand = BoostPriceOfCardsInHand;
            DeckServices.AddNew = AddNew;
            DeckServices.MoveCard = MoveCard;
            DeckServices.ReplaceCard = (card, data) => StartCoroutine(CardReplaceRoutine(card, data)); ;

            DeckServices.GetMatchingCards = GetMatchingCards;
            DeckServices.SetCustomDraw = (numDraw) => _customDraw = Mathf.Max(numDraw, _customDraw);


            EffectEvents.OnAdded += UpdateCardUI;
            EffectEvents.OnRemoved += UpdateCardUI;
            EffectEvents.OnUpdated += UpdateCardUI;
        }



        IEnumerator Start()
        {
            yield return FlowServices.WaitUntilEndOfSetup();
            yield return InventoryServices.WaitUntilReady();

            _drawPile = new DrawPile(_cardFactory);

            _cardPiles.Add(_drawPile);
            _cardPiles.Add(new CardPile(EnumCardPile.Discard, 0));
            _cardPiles.Add(new CardPile(EnumCardPile.Exhaust, 0));
            _cardPiles.Add(new CardPile(EnumCardPile.Effect, 0));

            for (int i = 0; i < _initialHandSize; i++)
            {
                _cardPiles.Add(new CardPile(EnumCardPile.Hand, i));
            }
            for (int i = 0; i < _initialSelectionSize; i++)
            {
                _cardPiles.Add(new CardPile(EnumCardPile.Selection, i));
            }
            _isReady = true;
        }

        private List<Card> GetMatchingCards(System.Predicate<Card> predicate, EnumCardPile pile)
        {
            return pile switch
            {
                EnumCardPile.Draw => _drawPile.Cards.FindAll(predicate),
                EnumCardPile.Discard => _discardPile.Cards.FindAll(predicate),
                EnumCardPile.Exhaust => _exhaustPile.Cards.FindAll(predicate),
                EnumCardPile.Hand => _handPiles.SelectMany(p => p.Cards).Where(c => predicate(c)).ToList(),

                _ => _cardPiles.FindAll(p => p.Type == pile).SelectMany(p => p.Cards).Where(c => predicate(c)).ToList(),
            };
        }

        private IEnumerator CardReplaceRoutine(Card card, ItemData data)
        {
            MoveCard(card, EnumCardPile.Effect, EnumPlacement.OnTop);
            yield return new WaitForSeconds(0.5f);

            Card newCard = DeckServices.AddNew(data,
                                    EnumCardPile.Effect,
                                    EnumPlacement.OnTop,
                                    EnumLifetime.Permanent);


            yield return DelayedMoveCard(newCard, EnumCardPile.Discard, EnumPlacement.OnTop, 2f);
            DeckServices.DestroyCard(card);
            yield return null;

            StartCoroutine(DrawBackToFull());

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
                    MoveToEffectPile(card, true);
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

            Card card = _cardFactory.Create(data);
            if (lifetime == EnumLifetime.Permanent)
                InventoryServices.AddItem(card.Item);

            card.Item.Lifetime = lifetime;

            MoveCard(card, pile, placement);
            return card;
        }

        private void BoostPriceOfCardsInHand(int bonus, System.Predicate<Card> predicate)
        {
            foreach (var pile in _handPiles)
            {
                if (pile.IsEmpty || !pile.Enabled) continue;

                if (predicate(pile.TopCard))
                {
                    pile.TopCard.InHandPriceBonus += bonus;
                    pile.TopCard.UpdateUI();
                }
            }
        }


        private Card CreateCard(ItemData data)
        {
            return _cardFactory.Create(data);
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

            StartCoroutine(PlayCardRoutine(_cardBeingPlayed, selectedPile));

        }


        private int GetCardPoolSize(EnumCardPile cardPileType)
        {
            if (cardPileType == EnumCardPile.Cart)
                return CartServices.GetCartSize();
            return _cardPiles.Count(p => p.Type == cardPileType && p.Enabled);
        }

        private Card GetTopCard(EnumCardPile type)
        {
            if (type == EnumCardPile.Draw)
                return _drawPile.TopCard;

            if (type == EnumCardPile.Discard)
                return _discardPile.TopCard;

            if (type == EnumCardPile.Cart)
                return CartServices.GetTopCard();

            var pile = _cardPiles.Find(p => p.Type == type);
            return pile?.TopCard;
        }


        private Card DuplicateCard(Card card)
        {
            if (card == null) return null;

            var duplicate = Instantiate(card);

            duplicate.name = card.name;
            duplicate.Item = card.Item;
            duplicate.OverrideCategory(card.Category);

            return duplicate;
        }


        #endregion

        #region Managing Effects 
        private void DestroyCard(Card card)
        {
            RemoveFromAllPiles(card);
            card.transform.DOKill();
            card.Destroy();

        }

        private void RestoreCardCategories()
        {
            foreach (var pile in _handPiles)
            {
                if (pile.IsEmpty || !pile.Enabled) continue;
                pile.RestoreCategory();
            }
            CartServices.RestoreCategories();

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
            CardPile impactedPile = null;

            foreach (var pile in _handPiles)
            {
                if (pile.IsEmpty) continue;
                if (pile.TopCard.Category == category) continue;

                impactedPile = pile;
                break;
            }
            if (impactedPile == null) return;

            var topCard = impactedPile.TopCard;
            if (topCard.Category == category) return;
            topCard.OverrideCategory(category);
        }


        private void UpdateCardUI(Effect _) => UpdateCardUI();
        private void UpdateCardUI()
        {

            _handPiles.ForEach(pile => pile.UpdateUI());
            _selectPiles.ForEach(pile => pile.UpdateUI());
        }


        #endregion




        #region Player Interations
        private void SelectCard(EnumCardPile type, int index)
        {
            DeactivateAllPiles();
            if (type == EnumCardPile.Hand)
                SelectCardFromHand(type, index);

            if (type == EnumCardPile.Selection)
                ClickOnCardSelection(type, index);
        }

        private void ClickOnCardSelection(EnumCardPile type, int index)
        {
            var pile = _cardPiles.Find(p => p.Type == type && p.Index == index);
            Assert.IsTrue(pile != null && !pile.IsEmpty, "called pile is null or Empty: " + type);

            var otherCards = new List<Card>();

            foreach (var selectPile in _selectPiles)
            {
                if (selectPile.Index == index) continue; // Skip the pile that was clicked

                if (selectPile != null && !selectPile.IsEmpty)
                    otherCards.Add(selectPile.TopCard);
            }
            DeckEvents.OnCardSelected(pile.TopCard, otherCards);
            ActivateTableCards();

        }

        private void SelectCardFromHand(EnumCardPile type, int index)
        {
            var pile = _cardPiles.Find(p => p.Type == type && p.Index == index);

            Assert.IsNotNull(pile,
                $"called pile is null or Empty: {type} - {index}");
            if (pile.IsEmpty) return;

            StartCoroutine(PlayCardRoutine(pile.TopCard, null));
        }

        private IEnumerator PlayCardRoutine(Card card, CardPile selectedPile)
        {
            var time = Time.time;
            DeckEvents.OnCardPlayed?.Invoke(card);

            MoveToEffectPile(card);

            yield return new WaitForSeconds(0.5f);

            yield return EffectServices.Execute(EnumEffectTrigger.OnCardPlayed, card);

            yield return EffectServices.AddEffectsFromCard(card);

            CartServices.AddToCartValue(card.Price);

            if (ExecuteSkills(card) ||
               (selectedPile == null && CartServices.AddCardToCart(card)) ||
                (selectedPile != null && CartServices.AddCardToCartPile(card, selectedPile)))
            {
                if (selectedPile != null && (selectedPile.TopCard == card))
                    yield return EffectServices.ExecutePile(EnumEffectTrigger.BeforeCartCalculation, selectedPile);

                yield return CartServices.CalculateCart();
            }
            else
            {
                Debug.LogWarning("Could not do anything with: " + card.Name);
                Discard(card);
            }

            _cardPlayed = true;
            _cardBeingPlayed = null;
            card.InHandPriceBonus = 0;
            card.UpdateUI();
            UpdateCardUI();
        }



        private void MoveToEffectPile(Card card, bool isInstant = false)
        {
            RemoveFromAllPiles(card);
            var effectPile = _cardPiles.Find(p => p.Type == EnumCardPile.Effect);

            effectPile.AddOnTop(card, isInstant);
        }

        private bool ExecuteSkills(Card card)
        {
            if (card.Category != EnumItemCategory.Skills) return false;

            Discard(card);
            return true;
        }




        #endregion


        #region Discarding Cards
        private void DiscardRandomCards(int numCard)
        {
            if (numCard < 0)
            {
                StartCoroutine(DiscardHand());
                return;
            }
            List<int> tablePileIndexes = new();
            for (int i = 0; i < _handPiles.Count; i++)
            {
                if (_handPiles[i].IsEmpty || !_handPiles[i].Enabled) continue;
                tablePileIndexes.Add(i);
            }
            tablePileIndexes.Shuffle();

            int numCardToDiscard = Mathf.Min(numCard, tablePileIndexes.Count);
            if (numCardToDiscard <= 0) return;
            for (int i = 0; i < numCardToDiscard; i++)
            {
                var pile = _handPiles[tablePileIndexes[i]];
                Discard(pile.TopCard);
            }
        }
        private void Discard(List<Card> cards)
        {

            foreach (var card in cards)
            {
                Discard(card);
            }
        }

        private void Discard(Card card)
        {
            if (_exhaustPile.Contains(card)) return;

            RemoveFromAllPiles(card);
            _discardPile.AddOnTop(card);
            card.InHandPriceBonus = 0;
            card.UpdateUI();
            card.Discard();

            //DeckEvents.OnCardMovedTo(card, EnumCardPile.Discard, _discardPile.Index, true);
        }

        private IEnumerator DiscardHand()
        {

            var cardsToDiscard = new List<Card>();
            foreach (var pile in _handPiles)
            {
                if (pile.IsEmpty || !pile.Enabled) continue;
                cardsToDiscard.AddRange(pile.Cards);
                yield return EffectServices.Execute(EnumEffectTrigger.OnDiscard, pile.TopCard);
                Discard(pile.TopCard);
                yield return new WaitForSeconds(0.2f);
            }

        }


        #endregion

        #region Card Pile Manipulation

        private void RemoveFromAllPiles(Card card)
        {
            _drawPile.RemoveCard(card);
            _discardPile.RemoveCard(card);
            foreach (var pile in _handPiles)
                pile.RemoveCard(card);

            CartServices.RemoveCard(card);


            foreach (var pile in _selectPiles)
                pile.RemoveCard(card);

        }

        private void ShuffleDiscardPileIn()
        {
            SetPileActivation(_handPiles, false);
            _drawPile.MergeBelow(_discardPile);
            _drawPile.Shuffle();
            DeckEvents.OnShuffle(EnumCardPile.Draw, _drawPile.Index, _drawPile.Cards);
        }

        private void DestroyEffemeralCards(CardPile pile)
        {
            var effemeralCards = pile.Cards.FindAll(c => c.Item.Lifetime == EnumLifetime.Effemeral);
            while (effemeralCards.Count > 0)
            {
                var card = effemeralCards[0];
                effemeralCards.RemoveAt(0);
                DestroyCard(card);
            }
        }

        private void ResetDecks()
        {
            DestroyEffemeralCards(_drawPile);
            DestroyEffemeralCards(_discardPile);
            DestroyEffemeralCards(_exhaustPile);


            _drawPile.MergeBelow(_exhaustPile);
            _drawPile.MergeBelow(_discardPile);

            _drawPile.Shuffle();
            DeckEvents.OnShuffle(EnumCardPile.Draw, _drawPile.Index, _drawPile.Cards);
        }

        private void AddCardToTable(Card card)
        {
            RemoveFromAllPiles(card);
            foreach (var pile in _handPiles)
            {
                if (pile.IsEmpty)
                {
                    pile.AddOnTop(card);

                    return; // Exit after adding to the first empty table pile
                }
            }

            Discard(card);
        }

        private void ShuffleDiscardIntoDrawPile()
        {
            if (_discardPile.IsEmpty) return;
            var cardsToMove = new List<Card>(_discardPile.Cards);
            foreach (var card in cardsToMove)
            {
                if (card == null) continue;
                _drawPile.AddAtTheBottom(card);
                _discardPile.RemoveCard(card);
                // DeckEvents.OnCardMovedTo(card, EnumCardPile.Draw, _drawPile.Index, false);
            }
            _drawPile.Shuffle();
        }


        private void ActivateTableCards()
        {
            foreach (var pile in _handPiles)
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
                else if (topCard.Category == EnumItemCategory.Curses)
                {
                    pile.Playable = false;
                }
                else
                {
                    pile.Playable = CartServices.CanAddToCart(topCard);
                }

                pile.Playable &= EffectServices.IsCardPlayable(topCard);
                DeckEvents.OnActivatePile(pile.Type, pile.Index, pile.Playable);
            }

        }


        private void MoveToCardSelectionPile(List<Card> list)
        {
            DeckEvents.OnCardsMovingToSelectPile();
            foreach (var card in list)
            {
                foreach (var pile in _selectPiles)
                {
                    if (pile.IsEmpty)
                    {
                        pile.AddOnTop(card);

                        break;
                    }
                }
            }

            //SetPileActivation(_selectPiles, false);
        }



        private void MovePileTo(EnumCardPile pileType1, EnumCardPile pileType2)
        {
            var pile1 = _cardPiles.Find(p => p.Type == pileType1);
            var pile2 = _cardPiles.Find(p => p.Type == pileType2);

            MovePileTo(pile1, pile2);

        }
        public static void MovePileTo(CardPile pile1, CardPile pile2)
        {
            if (pile1 == null || pile2 == null || pile1.IsEmpty) return;

            pile2.MergeBelow(pile1);
            DeckEvents.OnPileUpdated(pile1.Type, pile1.Index);
            DeckEvents.OnPileUpdated(pile2.Type, pile2.Index);
        }

        internal IEnumerator DrawBackToFull()
        {
            _cardPlayed = false;
            List<Card> drawnCards = new();

            if (_customDraw > 0)
            {
                drawnCards = _drawPile.DrawMany(_customDraw); ;
                DeckServices.MoveToCardSelect(drawnCards);
                DeckEvents.OnCardSelected += OnCardSelected;

                yield return new WaitUntil(() => _cardSelected != null);

                DeckEvents.OnCardSelected -= OnCardSelected;
                DeckServices.MoveToTable(_cardSelected);
                DeckServices.Discard(_otherCards);
                _customDraw = -1; // Reset custom draw after use
                _cardSelected = null;
                _otherCards = null;
            }

            int cardsNeeded = _handPiles.FindAll(p => p.IsEmpty && p.Enabled).Count;
            if (cardsNeeded <= 0)
            {
                // DeckServices.ActivateTableCards();
                yield break;
            }

            drawnCards = _drawPile.DrawMany(cardsNeeded);
            foreach (var card in drawnCards)
            {
                DeckServices.MoveToTable(card);
                //ADD THE OnDRAW Effect execution
                //EffectServices.Execute(EnumEffectTrigger.OnDraw, null);
                card.UpdateUI();
            }
            yield return new WaitForSeconds(drawnCards.Count * 0.2f);
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
                DeckEvents.OnActivatePile(pile.Type, pile.Index, activated);
            }
        }

        private void DeactivateAllPiles()
        {
            SetPileActivation(_handPiles, false);
            SetPileActivation(_selectPiles, false);
        }
        #endregion
    }
}
