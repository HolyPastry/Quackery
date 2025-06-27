using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holypastry.Bakery.Flow;
using Quackery.Inventories;
using System.Linq;
using Quackery.Effects;
using Quackery.Clients;
using Holypastry.Bakery;


namespace Quackery.Decks
{


    public class DeckManager : Service
    {
        [SerializeField] private Card _itemCardPrefab;
        [SerializeField] private Card _skillCardPrefab;
        [SerializeField] private Card _curseCardPrefab;

        private CardFactory _cardFactory;

        #region Service Properties
        private readonly List<CardPile> _piles = new()
        {
            new CardPile { Type = EnumPileType.DiscardPile },
            new CardPile { Type = EnumPileType.OnTable1 },
            new CardPile { Type = EnumPileType.OnTable2 },
            new CardPile { Type = EnumPileType.OnTable3 },
            new CardPile { Type = EnumPileType.OnTable4 },
            new CardPile { Type = EnumPileType.InCart1 },
            new CardPile { Type = EnumPileType.InCart2 },
            new CardPile { Type = EnumPileType.InCart3 },
            new CardPile { Type = EnumPileType.InCart4 },
            new CardPile { Type = EnumPileType.InCart5 },
            new CardPile { Type = EnumPileType.SelectPile1 },
            new CardPile { Type = EnumPileType.SelectPile2 },
            new CardPile { Type = EnumPileType.SelectPile3 },
            new CardPile { Type = EnumPileType.SelectPile4 }
        };
        private List<EnumPileType> _tablePileTypes = new()
        {
           EnumPileType.OnTable1 ,
           EnumPileType.OnTable2 ,
           EnumPileType.OnTable3 ,
            EnumPileType.OnTable4
        };

        private List<EnumPileType> _selectPileTypes = new()
        {
            EnumPileType.SelectPile1,
            EnumPileType.SelectPile2,
            EnumPileType.SelectPile3,
            EnumPileType.SelectPile4
        };

        private List<EnumPileType> _cartPileTypes = new()
        {
            EnumPileType.InCart1,
            EnumPileType.InCart2,
            EnumPileType.InCart3,
            EnumPileType.InCart4,
            EnumPileType.InCart5
        };
        private CardPile _lastCartPile;

        [SerializeField] private DrawPile _drawPile;
        private bool _movingCards;

        private CardPile _discardPile => _piles.Find(p => p.Type == EnumPileType.DiscardPile);
        private List<CardPile> _tablePiles => _piles.FindAll(p => _tablePileTypes.Contains(p.Type));
        private List<CardPile> _cartPiles => _piles.FindAll(p => _cartPileTypes.Contains(p.Type));
        private List<CardPile> _selectPiles => _piles.FindAll(p => _selectPileTypes.Contains(p.Type));

        public bool CartIsFull => _cartPiles.TrueForAll(p => !p.IsEmpty || !p.Enabled);

        public int CartSize => _cartPiles.Count(p => p.Enabled);

        #endregion


        #region Service Hookups

        void Awake()
        {
            _cardFactory = new CardFactory(_itemCardPrefab, _skillCardPrefab, _curseCardPrefab);
            _drawPile = new DrawPile(_cardFactory);
        }
        void OnDisable()
        {
            DeckServices.WaitUntilReady = () => new WaitUntil(() => true);
            DeckServices.WaitUntilCardStopMoving = () => new WaitUntil(() => true);

            DeckServices.AddToDrawPile = (itemData) => { };
            DeckServices.AddNewToDiscard = (itemData, amount) => { };
            DeckServices.AddNewToDrawDeck = (itemData, amount) => { };

            DeckServices.Shuffle = () => { };
            DeckServices.ShuffleDiscardIn = () => { };

            DeckServices.DrawBackToFull = () => { };
            DeckServices.Draw = (numberCards) => new List<Card>();
            DeckServices.InterruptDraw = () => { };
            DeckServices.DrawSpecificCards = (cards) => { };
            DeckServices.DrawCategory = (category) => null;

            DeckServices.DiscardHand = () => { };
            DeckServices.Discard = (card) => { };
            DeckServices.DiscardCart = () => { };
            DeckServices.DestroyCard = (cards) => { };
            DeckServices.DuplicateCard = (card) => null;
            DeckServices.DiscardCards = (amount) => { };


            DeckServices.DestroyCard = (cards) => { };


            DeckServices.GetPileRewards = (pileType) => new();
            DeckServices.GetTablePile = () => new();
            DeckServices.PileClicked = (pileType) => { };
            DeckServices.MovePileTo = (sourcePile, targetPile) => { };
            DeckServices.GetTopCard = (pileType) => null;
            DeckServices.EvaluatePileValue = (pileType) => 0;
            DeckServices.GetNumCardInCart = (category) => 0;
            DeckServices.GetCategoriesInCard = () => new();


            DeckServices.MoveToCardSelect = (cards) => { };
            DeckServices.MoveToTable = (card) => { };
            DeckServices.ReplaceTopCard = (pile) => { };


            DeckServices.IsCartFull = () => false;
            DeckServices.SetCartSize = (newSize) => { };
            DeckServices.ModifyCartSize = (amount) => { };

            DeckServices.MergeCart = (amount, category) => { };
            DeckServices.RecountCart = () => { };
            DeckServices.RecountPile = (pileType) => { };


            DeckServices.ChangeCardCategory = delegate { };
            DeckServices.RestoreCardCategories = () => { };


            EffectEvents.OnAdded -= CheckEffects;
            EffectEvents.OnRemoved -= CheckEffects;
            EffectEvents.OnUpdated -= CheckEffects;


        }

        void OnEnable()
        {
            DeckServices.WaitUntilReady = () => new WaitUntil(() => _isReady);
            DeckServices.WaitUntilCardStopMoving = () => new WaitUntil(() => !_movingCards);


            DeckServices.AddToDrawPile = _drawPile.AddNewCardToDeck;
            DeckServices.AddNewToDiscard = AddNewToDiscard;
            DeckServices.AddNewToDrawDeck = _drawPile.AddNew;


            DeckServices.Shuffle = ShuffleDiscardPileIn;
            DeckServices.ShuffleDiscardIn = ShuffleDiscardPileIn;

            DeckServices.DrawBackToFull = DrawBackToFull;
            DeckServices.Draw = _drawPile.DrawMany;
            DeckServices.InterruptDraw = () => _drawPile.InterruptDraw = true;
            DeckServices.DrawSpecificCards = _drawPile.DrawSpecificCards;
            DeckServices.DrawCategory = _drawPile.DrawCategoryCard;

            DeckServices.Discard = Discard;
            DeckServices.DiscardHand = DiscardHand;
            DeckServices.DiscardCart = DiscardCart;
            DeckServices.DestroyCard = DestroyCard;
            DeckServices.DuplicateCard = DuplicateCard;
            DeckServices.DiscardCards = DiscardRandomCards;

            DeckServices.PileClicked = PileClicked;
            DeckServices.GetPileRewards = GetPileRewards;
            DeckServices.GetTablePile = () => _tablePiles;
            DeckServices.MovePileTo = MovePileTo;
            DeckServices.GetTopCard = GetTopCard;
            DeckServices.EvaluatePileValue = (pileType) => GetPileRewards(pileType).Sum(r => r.Value);
            DeckServices.GetNumCardInCart = GetNumCardInCart;
            DeckServices.GetCategoriesInCard = GetCategoriesInCard;

            DeckServices.MoveToCardSelect = MoveToCardSelectionPile;
            DeckServices.MoveToTable = AddCardToTable;
            DeckServices.ReplaceTopCard = ReplaceTopCard;
            DeckServices.ActivateTableCards = ActivateTableCards;

            DeckServices.SetCartSize = (newSize) => UpdateCartSize(newSize);
            DeckServices.ModifyCartSize = (amount) => UpdateCartSize(Mathf.Max(2, CartSize + amount));
            DeckServices.IsCartFull = () => CartIsFull;


            DeckServices.MergeCart = MergeCart;
            DeckServices.RecountCart = RecountCart;
            DeckServices.RecountPile = RecountPile;


            DeckServices.ChangeCardCategory = ChangeCardCategory;
            DeckServices.RestoreCardCategories = RestoreCardCategories;


            EffectEvents.OnAdded += CheckEffects;
            EffectEvents.OnRemoved += CheckEffects;
            EffectEvents.OnUpdated += CheckEffects;
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

        private Card GetTopCard(EnumPileType type)
        {
            if (type == EnumPileType.DrawPile)
                return _drawPile.TopCard;

            if (type == EnumPileType.DiscardPile)
                return _discardPile.TopCard;

            if (type == EnumPileType.InCart)
                return _lastCartPile?.TopCard;

            var pile = _piles.Find(p => p.Type == type);
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

        private List<EnumItemCategory> GetCategoriesInCard()
        {
            var categories = new List<EnumItemCategory>();
            foreach (var pile in _cartPiles)
            {
                if (!pile.Enabled || pile.IsEmpty) continue;
                foreach (var card in pile.Cards)
                {
                    if (card.Category != EnumItemCategory.Unset)
                        categories.AddUnique(card.Category);
                }
            }
            return categories.ToList();
        }

        private void ReplaceTopCard(CardPile pile)
        {
            if (_lastCartPile == null ||
                _lastCartPile.IsEmpty ||
                _lastCartPile.Count < 2)
                return;

            var topCard = _lastCartPile.TopCard;
            var secondCard = _lastCartPile.Cards[1];

            _lastCartPile.RemoveCard(secondCard);
            AddCardToTable(secondCard);
            DeckEvents.OnPileUpdated(_lastCartPile.Type);

        }

        private int GetNumCardInCart(EnumItemCategory category)
        {
            int numCards = 0;
            foreach (var pile in _cartPiles)
            {
                if (!pile.Enabled || pile.IsEmpty) continue;
                numCards += pile.Cards.Count(card => card.Category == category || category == EnumItemCategory.Unset);
            }
            return numCards;
        }

        protected override IEnumerator Start()
        {
            yield return FlowServices.WaitUntilReady();
            yield return InventoryServices.WaitUntilReady();
            var allItems = InventoryServices.GetAllItems();
            _drawPile.Populate(allItems);
            _isReady = true;
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
            foreach (var pile in _tablePiles)
            {
                if (pile.IsEmpty || !pile.Enabled) continue;
                pile.RestoreCategory();
            }
            foreach (var pile in _cartPiles)
            {
                if (pile.IsEmpty || !pile.Enabled) continue;
                pile.RestoreCategory();
            }
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
                foreach (var pile in _cartPiles)
                {
                    if (pile.IsEmpty || !pile.Enabled) continue;
                    pile.OverrideStackCategory(category);
                    DeckEvents.OnPileUpdated(pile.Type);
                }
                return;
            }

        }
        private void ChangeRandomCardCategoryOnTable(EnumItemCategory category)
        {
            CardPile impactedPile = null;

            foreach (var pile in _tablePiles)
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

        private void RecountCart()
        {
            foreach (var pile in _cartPiles)
            {
                if (pile.IsEmpty || !pile.Enabled) continue;
                DeckEvents.OnCashingTheCart(pile.Type);
            }
        }

        private void RecountPile(CardPile pile)
        {
            if (pile.IsEmpty || !pile.Enabled) return;
            DeckEvents.OnCashingPile(pile);
        }

        private void DestroyPile(EnumPileType type)
        {
            var pile = _piles.Find(p => p.Type == type);
            pile?.Clear();
            DeckEvents.OnPileDestroyed(type);
        }

        private void UpdateCartSize(int cartSize)
        {
            for (int i = 0; i < _cartPiles.Count; i++)
            {
                _cartPiles[i].Enabled = i < cartSize;
            }
            DeckEvents.OnCartSizeUpdated(cartSize);
        }

        private void CheckEffects(Effect _)
        {
            _cartPiles.ForEach(pile =>
            {
                if (pile.IsEmpty || !pile.Enabled) return;
                pile.TopCard.UpdateUI();
            });
            _tablePiles.ForEach(pile =>
            {
                if (pile.IsEmpty || !pile.Enabled) return;
                pile.TopCard.UpdateUI();
            });
            _selectPiles.ForEach(pile =>
            {
                if (pile.IsEmpty || !pile.Enabled) return;
                pile.TopCard.UpdateUI();
            });
        }

        private void MergeCart(int amount, EnumItemCategory category)
        {
            int index = _lastCartPile != null ? _cartPiles.IndexOf(_lastCartPile) : -1;
            if (index == -1 || index == 0) return; // No other pile to merge into

            if (category == EnumItemCategory.Unset)
            {
                while (CartPilesHaveSameCategory(out List<CardPile> piles))
                {
                    for (int i = 1; i < piles.Count; i++)
                        MovePileTo(piles[i], piles[0]);
                }
            }
            else
            {
                CardPile firstPile = null;
                List<CardPile> pilesToMerge = new();
                for (int i = 0; i <= _cartPiles.Count; i++)
                {
                    if (_cartPiles[i].IsEmpty ||
                        !_cartPiles[i].Enabled ||
                        _cartPiles[i].Category != category) continue;

                    if (firstPile == null)
                        firstPile = _cartPiles[i];
                    else
                        pilesToMerge.Add(_cartPiles[i]);
                }
                foreach (var pile in pilesToMerge)
                {
                    MovePileTo(pile, firstPile);
                }
            }

            _lastCartPile = _cartPiles[index - 1];
            DeckEvents.OnCashingTheCart(_lastCartPile.Type);
        }

        private bool CartPilesHaveSameCategory(out List<CardPile> piles)
        {
            //piles = _cartPiles.FindAll(p => p.Enabled && !p.IsEmpty);
            piles = new List<CardPile>();
            for (int i = 0; i < _cartPiles.Count; i++)
            {
                if (!_cartPiles[i].Enabled || _cartPiles[i].IsEmpty) continue;
                for (int j = i + 1; j < _cartPiles.Count; j++)
                {
                    if (!_cartPiles[j].Enabled || _cartPiles[j].IsEmpty) continue;
                    if (_cartPiles[i].Category != _cartPiles[j].Category) continue;
                    piles.AddUnique(_cartPiles[i]);
                    piles.AddUnique(_cartPiles[j]);
                    return true; // Found at least two piles with the same category
                }
            }
            return piles.Count >= 2;
        }

        #endregion


        private List<CardReward> GetPileRewards(EnumPileType type)
        {
            var pile = _piles.Find(p => p.Type == type);
            if (pile == null || pile.IsEmpty) return new();
            int index = _cartPiles.IndexOf(pile);
            List<CardPile> otherCartPiles = _cartPiles.Where((p, i) => i != index).ToList();

            return pile.CalculateCartRewards(otherCartPiles);
        }

        #region Player Interations
        private void PileClicked(EnumPileType type)
        {
            DeactivateAllPiles();
            if (_tablePileTypes.Contains(type))
                ClickOnTablePile(type);

            if (_selectPileTypes.Contains(type))
                ClickOnCardSelection(type);
        }

        private void ClickOnCardSelection(EnumPileType type)
        {
            var pile = _piles.Find(p => p.Type == type);
            if (pile == null || pile.IsEmpty) return;

            var otherCards = new List<Card>();
            foreach (var selectPile in _selectPileTypes)
            {
                if (selectPile == type) continue; // Skip the clicked pile
                var otherPile = _piles.Find(p => p.Type == selectPile);
                if (otherPile != null && !otherPile.IsEmpty)
                    otherCards.Add(otherPile.TopCard);
            }
            DeckEvents.OnCardSelected(pile.TopCard, otherCards);
            ActivateTableCards();

        }

        private void ClickOnTablePile(EnumPileType type)
        {

            var pile = _piles.Find(p => p.Type == type);
            if (pile == null || pile.IsEmpty) return;

            var meDialog = pile.TopCard.Item.Data.name + "Me";
            string clientResponse = ClientServices.SelectedClient().Data.CharacterData.name + "Answer";
            DialogQueueServices.QueueDialog(meDialog);
            DialogQueueServices.QueueDialog(clientResponse);

            if (ExecuteSkills(pile)) return;

            if (ActivatePreviousCard(pile)) return;

            if (MergeWithPrevious(pile)) return;

            MergePileToCart(pile);
        }


        private bool ExecuteSkills(CardPile pile)
        {
            if (pile.TopCard.Category != EnumItemCategory.Skills) return false;
            pile.TopCard.ExecutePowerInCart(pile);
            CardGameContollerServices.ModifyCartCash(pile.TopCard.Price);
            Discard(pile.TopCard);
            DeckServices.DrawBackToFull();

            return true;
        }

        private bool ActivatePreviousCard(CardPile pile)
        {
            return false;
            if (_lastCartPile != null && !_lastCartPile.IsEmpty &&
                // _lastCartPile.TopCard.HasActivatableEffects &&
                _lastCartPile.Category == pile.TopCard.Category)
            {
                // If the last cart pile is not empty and has the same category, merge into it
                _lastCartPile.MergeBelow(pile);
                DeckEvents.OnPileMoved(_lastCartPile.Type);
                DeckEvents.OnCashingTheCart(_lastCartPile.Type);
                _lastCartPile.TopCard.ActivatePower(_lastCartPile);
                return true; // Exit after merging to the last cart pile
            }

            return false;
        }

        private bool MergeWithPrevious(CardPile pile)
        {
            var topCard = pile.TopCard;

            var effect = topCard.Effects.Find(effect => effect.Data is MergeWithPreviousPileEffect);
            if (effect == null) return false;

            var effectData = effect.Data as MergeWithPreviousPileEffect;
            CardPile cardPileRef = null;

            if (effectData.TargetStack == MergeWithPreviousPileEffect.EnumTargetStack.Previous)
            {
                if (_lastCartPile == null || _lastCartPile.IsEmpty)
                {
                    Debug.LogWarning(
                        "MergeWithPreviousPileEffect: No last cart pile to merge with. This card should not be playable");
                    return false;
                }
                cardPileRef = _lastCartPile;
            }
            else if (effectData.TargetStack == MergeWithPreviousPileEffect.EnumTargetStack.LowestValue)
            {
                cardPileRef = _cartPiles
                    .Where(p => p.Enabled && !p.IsEmpty)
                    .OrderBy(p => GetPileRewards(p.Type).Sum(r => r.Value))
                    .FirstOrDefault();
            }
            if (cardPileRef == null || cardPileRef.IsEmpty)
            {
                Debug.LogWarning("MergeWithPreviousPileEffect: No valid previous pile found. This card should not have been playable");
                return false;
            }

            if (effectData.Category != EnumItemCategory.Unset &&
               effectData.Category != cardPileRef.Category)
            {
                Debug.LogWarning("CAtegory is not matching: No valid previous pile found. This card should not have been playable");
                return false;
            }

            if (effectData.Location == EnumPileLocation.AtTheBottom)
            {
                cardPileRef.MergeBelow(pile);
                DeckEvents.OnPileMoved(cardPileRef.Type);
                DeckEvents.OnCashingPile(cardPileRef);
                CheckEffects(null);
                return true;
            }
            else if (effectData.Location == EnumPileLocation.OnTop)
            {
                cardPileRef.MergeOnTop(pile);
                cardPileRef.TopCard.ExecutePowerInCart(cardPileRef);
                DeckEvents.OnPileMoved(cardPileRef.Type);
                DeckEvents.OnCashingTheCart(cardPileRef.Type);
                CheckEffects(null);
                return true; // Exit after merging to the last cart pile
            }
            else
            {
                Debug.LogWarning($"MergeWithPreviousPileEffect: Unknown location {effectData.Location} for card {topCard.Item.Data.name}");
            }

            return false;
        }
        private void MergePileToCart(CardPile pile)
        {
            var cartPile = _cartPiles.Find(c => c.IsEmpty && c.Enabled);
            if (cartPile == null) return;

            var topCard = pile.TopCard;
            cartPile.MergeBelow(pile);
            _lastCartPile = cartPile;
            topCard.ExecutePowerInCart(cartPile);
            DeckEvents.OnPileMoved(cartPile.Type);
            DeckEvents.OnCashingTheCart(cartPile.Type);
            CheckEffects(null);
        }
        #endregion


        #region Discarding Cards
        private void DiscardRandomCards(int numCard)
        {
            List<int> tablePileIndexes = new();
            for (int i = 0; i < _tablePiles.Count; i++)
            {
                if (_tablePiles[i].IsEmpty || !_tablePiles[i].Enabled) continue;
                tablePileIndexes.Add(i);
            }
            tablePileIndexes.Shuffle();

            int numCardToDiscard = Mathf.Min(numCard, tablePileIndexes.Count);
            if (numCardToDiscard <= 0) return;
            for (int i = 0; i < numCardToDiscard; i++)
            {
                var pile = _tablePiles[tablePileIndexes[i]];
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
            _discardPile.AddAtTheTop(card);
            card.Discard();

            DeckEvents.OnCardMovedTo(card, EnumPileType.DiscardPile, true);
        }

        private void DiscardHand()
        {

            var cardsToDiscard = new List<Card>();
            foreach (var pile in _tablePiles)
            {
                cardsToDiscard.AddRange(pile.Cards);
            }
            foreach (var card in cardsToDiscard)
            {
                Discard(card);
            }
        }


        #endregion

        #region Card Pile Manipulation

        private void RemoveFromAllPiles(Card card)
        {
            _drawPile.RemoveCard(card);
            _discardPile.RemoveCard(card);
            foreach (var pile in _tablePiles)
                pile.RemoveCard(card);

            foreach (var pile in _cartPiles)
                pile.RemoveCard(card);

            foreach (var pile in _selectPiles)
                pile.RemoveCard(card);

        }

        private void ShuffleDiscardPileIn()
        {
            SetPileActivation(_tablePiles, false);
            _drawPile.MergeBelow(_discardPile);
            _drawPile.Shuffle();
            DeckEvents.OnShuffle(EnumPileType.DrawPile, _drawPile.Cards);
        }

        private void AddCardToTable(Card card)
        {
            RemoveFromAllPiles(card);
            foreach (var pile in _tablePiles)
            {
                if (pile.IsEmpty)
                {
                    pile.AddAtTheTop(card);
                    DeckEvents.OnCardMovedTo(card, pile.Type, true);
                    return; // Exit after adding to the first empty table pile
                }
            }
            // If no empty table pile found, discard the card
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
                DeckEvents.OnCardMovedTo(card, EnumPileType.DrawPile, false);
            }
            _drawPile.Shuffle();
        }


        private void ActivateTableCards()
        {
            foreach (var pile in _tablePiles)
            {
                if (pile.IsEmpty) continue;
                var topCard = pile.TopCard;
                pile.Playable = false;
                if (topCard.Category == EnumItemCategory.Skills)
                {
                    if (topCard.Price > 0) pile.Playable = true;
                    else
                        pile.Playable = CardGameContollerServices.CanCartAfford(-topCard.Price);
                }
                else if (topCard.Category == EnumItemCategory.Fatigues)
                {
                    pile.Playable = false;
                }
                else
                {
                    pile.Playable = CanAddToCart(topCard);
                }

                pile.Playable &= EffectServices.IsCardPlayable(topCard);
                DeckEvents.OnActivatePile(pile.Type, pile.Playable);
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
                        pile.AddAtTheTop(card);
                        DeckEvents.OnCardMovedTo(card, pile.Type, true);
                        break;
                    }
                }
            }
            SetPileActivation(_selectPiles, true);
        }

        private void DiscardCart()
        {
            EffectServices.RemoveEffectsLinkedToPiles(_cartPiles);
            List<Card> cardsToDiscard = new();
            foreach (var cartPile in _cartPiles)
                foreach (var card in cartPile.Cards)
                    cardsToDiscard.Add(card);

            cardsToDiscard.ForEach(card => Discard(card));

            _lastCartPile = null;
        }

        private void MovePileTo(EnumPileType pileType1, EnumPileType pileType2)
        {
            var pile1 = _piles.Find(p => p.Type == pileType1);
            var pile2 = _piles.Find(p => p.Type == pileType2);

            MovePileTo(pile1, pile2);

        }
        private void MovePileTo(CardPile pile1, CardPile pile2)
        {
            if (pile1 == null || pile2 == null || pile1.IsEmpty) return;

            pile2.MergeBelow(pile1);
            DeckEvents.OnPileMoved(pile2.Type);
        }

        internal void DrawBackToFull()
        {
            EffectServices.Execute(EnumEffectTrigger.OnDraw, null);

            if (_drawPile.InterruptDraw)
            {
                _drawPile.InterruptDraw = false;
                return;
            }
            int cardsNeeded = _tablePiles.FindAll(p => p.IsEmpty).Count;
            if (cardsNeeded <= 0)
            {
                DeckServices.ActivateTableCards();
                return;
            }

            List<Card> drawnCards = _drawPile.DrawMany(cardsNeeded);
            foreach (var card in drawnCards)
            {
                DeckServices.MoveToTable(card);
                card.UpdateUI();
            }
            DeckServices.ActivateTableCards();
        }


        private bool CanAddToCart(Card topCard)
        {

            var mergeEffects = topCard.Effects.FindAll(effect => effect.Data is MergeWithPreviousPileEffect);
            if (mergeEffects.Count == 0)
                return !CartIsFull;
            // If there are merge effects, check if the last cart pile is compatible
            if (_lastCartPile == null || _lastCartPile.IsEmpty) return false;
            var mergeEffect = mergeEffects[0].Data as MergeWithPreviousPileEffect;

            if (mergeEffect.Location == EnumPileLocation.OnTop &&
                 _lastCartPile.TopCard.CannotBeCovered)
                return false;
            if (mergeEffect.Category != EnumItemCategory.Unset &&
                _lastCartPile.Category != mergeEffect.Category) return false;
            return true;
        }

        private void SetPileActivation(List<CardPile> piles, bool activated)
        {
            foreach (var pile in piles)
            {
                DeckEvents.OnActivatePile(pile.Type, activated);
            }
        }

        private void DeactivateAllPiles()
        {
            SetPileActivation(_tablePiles, false);
            SetPileActivation(_selectPiles, false);
        }
        #endregion
    }
}
