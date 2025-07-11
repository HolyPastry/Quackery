using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Quackery.Inventories;
using System.Linq;
using Quackery.Effects;

using UnityEngine.Assertions;


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



        private bool _movingCards;
        private bool _cardPlayed;

        private DrawPile _drawPile;
        private bool _drawInterrupted;
        private bool _isReady;
        private Card _cardBeingPlayed;

        private CardPile _discardPile => _cardPiles.Find(p => p.Type == EnumCardPile.Discard);
        private List<CardPile> _handPiles => _cardPiles.FindAll(p => p.Type == EnumCardPile.Hand);
        private List<CardPile> _selectPiles => _cardPiles.FindAll(p => p.Type == EnumCardPile.Selection);

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

            DeckServices.InterruptDraw = () => { };
            DeckServices.ResumeDraw = () => { };

            DeckServices.GetTablePile = () => new();
            DeckServices.PileClicked = (pileType, index) => { };
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
            DeckServices.MoveCardToEffect = (card, teleport) => { };
            DeckServices.CreateCard = (itemData) => null;

            DeckServices.PopulateDeck = () => { };

            DeckServices.StartPlayCardLoop = delegate { };
            DeckServices.StopPlayCardLoop = () => { };

            DeckServices.BoostPriceOfCardsInHand = (value, predicate) => { };
            DeckServices.AddNew =
                (itemData, pileType, pileLocation, lifetime, sendToEffectPileFirst) => null;

            DeckServices.ReplaceCard = (card, replacementCard) => { };

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

            DeckServices.PopulateDeck = PopulateDeck;

            DeckServices.InterruptDraw = () => _drawInterrupted = true;
            DeckServices.ResumeDraw = () => { _drawInterrupted = false; _cardPlayed = true; };


            DeckServices.PileClicked = PileClicked;
            DeckServices.GetTablePile = () => _handPiles;
            DeckServices.MovePileType = MovePileTo;
            DeckServices.GetTopCard = GetTopCard;

            DeckServices.MoveToCardSelect = MoveToCardSelectionPile;
            DeckServices.MoveToTable = AddCardToTable;
            DeckServices.MoveCardToEffect = MoveToEffectPile;
            DeckServices.ActivateTableCards = ActivateTableCards;

            DeckServices.ChangeCardCategory = ChangeCardCategory;
            DeckServices.RestoreCardCategories = RestoreCardCategories;


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
            DeckServices.ReplaceCard = ReplaceACard;

            EffectEvents.OnAdded += UpdateCardUI;
            EffectEvents.OnRemoved += UpdateCardUI;
            EffectEvents.OnUpdated += UpdateCardUI;
        }

        private void ReplaceACard(Card card, ItemData data)
        {
            StartCoroutine(CardReplaceRoutine(card, data));
        }

        private IEnumerator CardReplaceRoutine(Card card, ItemData data)
        {
            DeckServices.DestroyCard(card);

            yield return StartCoroutine(DeckServices.AddNew(data,
                                   EnumCardPile.Discard,
                                   EnumPlacement.OnTop,
                                   EnumLifetime.Permanent, true));


            DeckServices.DrawBackToFull();
        }

        private IEnumerator AddNew(ItemData data,
                         EnumCardPile pile,
                          EnumPlacement placement,
                           EnumLifetime lifetime,
                           bool sendToEffectPileFirst = false)
        {
            if (data == null) yield break;

            Card card = _cardFactory.Create(data);
            if (lifetime == EnumLifetime.Permanent)
                InventoryServices.AddItem(card.Item);

            card.Item.Lifetime = lifetime;

            if (sendToEffectPileFirst)
            {
                MoveToEffectPile(card, true);
                yield return new WaitForSeconds(1f);
            }

            switch (pile)
            {
                case EnumCardPile.Draw:
                    _drawPile.Add(card, placement);
                    break;
                case EnumCardPile.Discard:
                    _discardPile.Add(card, placement);
                    break;
                default:
                    Debug.LogWarning("Cannot Add new cart to: " + pile);
                    break;
            }
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

        private void PopulateDeck()
        {

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
        private void StopPlayCardLoop()
        {

            var selectedPile = CartServices.GetSelectedPile();
            if (selectedPile == null) return;
            CartServices.SetStacksHighlights(null);
            DeactivateAllPiles();
            StartCoroutine(PlayCardRoutine(_cardBeingPlayed, selectedPile));

        }

        private void StartPlayCardLoop(Card card)
        {
            _cardBeingPlayed = card;
            CartServices.SetStacksHighlights(card);


        }
        private int GetCardPoolSize(EnumCardPile cardPileType)
        {
            if (cardPileType == EnumCardPile.Cart)
                return CartServices.GetCartSize();
            return _cardPiles.Count(p => p.Type == cardPileType && p.Enabled);
        }

        private void AddNewToDiscard(ItemData data, int numCards)
        {
            for (int i = 0; i < numCards; i++)
            {
                Item item = InventoryServices.AddNewItem(data);
                Card card = _cardFactory.Create(item);
                Discard(card);
            }
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

        private void AddToDiscard(Card card)
        {
            if (card == null) return;
            Discard(card);
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



        private void RecountPile(CardPile pile)
        {
            if (pile.IsEmpty || !pile.Enabled) return;
        }

        private void DestroyPile(EnumCardPile type)
        {
            var pile = _cardPiles.Find(p => p.Type == type);
            if (pile == null) return;
            pile?.Clear();
            DeckEvents.OnPileDestroyed(type, pile.Index);
        }





        private void UpdateCardUI(Effect _) => UpdateCardUI();
        private void UpdateCardUI()
        {

            _handPiles.ForEach(pile => pile.UpdateUI());

            _selectPiles.ForEach(pile => pile.UpdateUI());
        }


        #endregion




        #region Player Interations
        private void PileClicked(EnumCardPile type, int index)
        {
            DeactivateAllPiles();
            if (type == EnumCardPile.Hand)
                ClickOnTablePile(type, index);

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

        private void ClickOnTablePile(EnumCardPile type, int index)
        {
            var pile = _cardPiles.Find(p => p.Type == type && p.Index == index);
            Assert.IsTrue(pile != null && !pile.IsEmpty,
                $"called pile is null or Empty: {type} - {index}");

            StartCoroutine(PlayCardRoutine(pile.TopCard));

        }

        private IEnumerator PlayCardRoutine(Card card, CardPile selectedPile)
        {
            DeckEvents.OnCardPlayed?.Invoke(card);

            RemoveFromAllPiles(card);

            yield return StartCoroutine(OnPlayEffectRoutine(card));

            CartServices.AddToCartValue(card.Price);
            if (ExecuteSkills(card))
            {

            }
            else if (CartServices.AddCardToCartPile(card, selectedPile))
            {
                yield return CartServices.CalculateCart();
            }
            else
            {
                Debug.LogWarning("Could not do anything with: " + card.Name);
                Discard(card);
            }
            if (!_drawInterrupted)
                _cardPlayed = true;
            _cardBeingPlayed = null;
            card.InHandPriceBonus = 0;
            card.UpdateUI();
            UpdateCardUI();
        }

        private IEnumerator PlayCardRoutine(Card card)
        {

            DeckEvents.OnCardPlayed?.Invoke(card);
            // var meDialog = card.Item.Data.name + "Me";
            // string clientResponse = ClientServices.SelectedClient().DialogName + "Answer";
            // DialogQueueServices.QueueDialog(meDialog);
            // DialogQueueServices.QueueDialog(clientResponse);

            RemoveFromAllPiles(card);

            yield return StartCoroutine(OnPlayEffectRoutine(card));


            CartServices.AddToCartValue(card.Price);
            if (ExecuteSkills(card))
            {

            }

            else if (CartServices.AddCardToCart(card))
            {
                yield return CartServices.CalculateCart();

            }
            else
            {

                Debug.LogWarning("Could not do anything with: " + card.Name);
                Discard(card);
            }
            if (!_drawInterrupted)
                _cardPlayed = true;
            UpdateCardUI();

        }

        private IEnumerator OnPlayEffectRoutine(Card card)
        {
            Debug.Log(card);
            var onPlayEffects = card.Effects.FindAll(e => e != null && e.Trigger == EnumEffectTrigger.OnCardPlayed ||
                                                            e.Tags.Contains(EnumEffectTag.Status)
                                                             || e.Trigger == EnumEffectTrigger.OnActivated);
            if (onPlayEffects.Count == 0)
                yield break;

            MoveToEffectPile(card);
            yield return new WaitForSeconds(0.5f);

            for (int i = 0; i < onPlayEffects.Count; i++)
            {
                if (onPlayEffects[i].Tags.Contains(EnumEffectTag.Status))
                {
                    onPlayEffects[i].LinkedCard = card;
                    EffectServices.AddEffect(onPlayEffects[i]);
                }
                else if (onPlayEffects[i].Trigger == EnumEffectTrigger.OnActivated)
                {
                    onPlayEffects[i].Tags.Add(EnumEffectTag.Status);
                    onPlayEffects[i].Tags.Add(EnumEffectTag.Card);
                    onPlayEffects[i].LinkedCard = card;
                    EffectServices.AddEffect(onPlayEffects[i]);
                }
                else onPlayEffects[i].Execute(card);
                yield return new WaitForSeconds(0.5f);
            }
        }


        private void MoveToEffectPile(Card card, bool isInstant = false)
        {
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
                EffectServices.Execute(EnumEffectTrigger.OnDiscard, pile.TopCard);
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
            SetPileActivation(_selectPiles, true);
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
        }

        internal IEnumerator DrawBackToFull()
        {
            _cardPlayed = false;

            int cardsNeeded = _handPiles.FindAll(p => p.IsEmpty && p.Enabled).Count;
            if (cardsNeeded <= 0)
            {
                // DeckServices.ActivateTableCards();
                yield break;
            }

            List<Card> drawnCards = _drawPile.DrawMany(cardsNeeded);
            foreach (var card in drawnCards)
            {
                DeckServices.MoveToTable(card);
                //ADD THE OnDRAW Effect execution
                //EffectServices.Execute(EnumEffectTrigger.OnDraw, null);
                card.UpdateUI();
            }
            yield return new WaitForSeconds(drawnCards.Count * 0.2f);
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
