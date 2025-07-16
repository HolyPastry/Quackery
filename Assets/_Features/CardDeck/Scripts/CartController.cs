using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Holypastry.Bakery;
using Quackery.Clients;
using Quackery.Effects;
using Quackery.Inventories;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Quackery.Decks
{
    [Serializable]
    public struct CartEvaluation
    {
        public int Index;
        public float Value;
        public Color Color;
        public string Description;
    }
    public class CartController : MonoBehaviour
    {

        [SerializeField] private int _initialCartSize = 3;
        [SerializeField]
        private List<CartEvaluation> _cartEvaluations = new()
        {
            new () { Index = 0, Value = 0.8f, Color = Color.yellow, Description = "Amazing" },
            new () {  Index = 1,Value = 0.6f, Color = Color.cyan, Description = "Good" },
            new () {  Index = 2,Value = 0.4f, Color = Color.green, Description = "Poor" },
            new () {  Index = 3,Value = 0.2f, Color = Color.blue, Description = "Terrible"  },
            new () {  Index = 4, Value = 0.0f, Color = Color.red, Description = "Catastrophic" }
        };


        private int _ratingCartSizeModifier;
        private int _cardCartSizeModifier;
        private int _cartBonus;
        private int _cartValue;

        private readonly List<CardPile> _cartPiles = new();

        public bool CartIsFull => _cartPiles.TrueForAll(p => !p.IsEmpty || !p.Enabled);

        public int CartSize => _cartPiles.Count(p => p.Enabled);

        public int CartValue
        {
            get => _cartValue;
            private set
            {
                _cartValue = value;
                CartEvents.OnCartValueChanged?.Invoke();
            }
        }

        private CardPile _lastCartPile;

        private Card _cardBeingPlayed;
        private CardPile _hoveredPile;

        void OnEnable()
        {

            CartServices.SetRatingCartSizeModifier = SetRatingModifier;
            CartServices.GetCartSize = () => CartSize;

            CartServices.DiscardCart = DiscardCart;
            CartServices.MergeCart = MergeCart;

            CartServices.GetNumCardInCart = GetNumCardInCart;
            CartServices.GetCategoriesInCart = GetCategoriesInCart;

            CartServices.AddCardToCart = AddCardToCart;
            CartServices.AddCardToCartPile = AddCardToCartPile;
            CartServices.CanAddToCart = CanAddToCart;

            CartServices.RestoreCategories = RestoreCategories;
            CartServices.ChangeCardCategory = ChangeCardCategory;
            CartServices.RemoveCard = RemoveCard;

            CartServices.CalculateCart = CalculateCart;
            CartServices.GetCartBonus = GetCartBonus;


            CartServices.AddToCartValue = value => CartValue += value;
            CartServices.SetCartValue = value => CartValue = value;
            CartServices.GetCartValue = () => CartValue;
            CartServices.CanCartAfford = value => CartValue >= value;


            CartServices.ReplaceTopCard = ReplaceTopCard;
            CartServices.GetTopCard = GetTopCard;

            CartServices.ValidateCart = Validate;

            CartServices.SetStacksHighlights = SetStacksHighlights;
            CartServices.GetHoveredPile = GetSelectedPile;
            CartServices.HoverPile = HoverPile;
            CartServices.UnhoverPile = UnhoverPile;


            CartServices.RandomizeCart = RandomizeCart;
            CartServices.GetCartEvaluation = GetCartEvaluation;


            EffectEvents.OnAdded += UpdateCardUI;
            EffectEvents.OnRemoved += UpdateCardUI;
            EffectEvents.OnUpdated += UpdateCardUI;

        }



        void OnDisable()
        {
            CartServices.SetRatingCartSizeModifier = delegate { };

            CartServices.GetCartSize = () => 0;

            CartServices.DiscardCart = delegate { };
            CartServices.MergeCart = delegate { };

            CartServices.GetNumCardInCart = (category) => 0;
            CartServices.GetCategoriesInCart = () => new List<EnumItemCategory>();
            CartServices.CalculateCart = () => null;

            CartServices.AddCardToCart = (pile) => false;
            CartServices.CanAddToCart = (card) => true;

            CartServices.RestoreCategories = delegate { };
            CartServices.ChangeCardCategory = delegate { };

            CartServices.RemoveCard = delegate { };


            CartServices.AddToCartValue = value => { };
            CartServices.SetCartValue = value => { };
            CartServices.GetCartValue = () => 0;
            CartServices.CanCartAfford = value => true;

            CartServices.ReplaceTopCard = delegate { };
            CartServices.GetTopCard = () => null;

            CartServices.ValidateCart = () => { };

            CartServices.SetStacksHighlights = delegate { };
            CartServices.GetHoveredPile = () => null;
            CartServices.HoverPile = (index) => { };
            CartServices.UnhoverPile = (index) => { };
            // CartServices.AddCardToCartPile = (card, pile) => true;
            CartServices.AddCardToCartPile = (card, pile) => false;

            CartServices.RandomizeCart = delegate { };
            CartServices.GetCartEvaluation = () => default;

            EffectEvents.OnAdded -= UpdateCardUI;
            EffectEvents.OnRemoved -= UpdateCardUI;
            EffectEvents.OnUpdated -= UpdateCardUI;
        }

        IEnumerator Start()
        {
            yield return FlowServices.WaitUntilReady();
            _cartPiles.Clear();
            for (int i = 0; i < _initialCartSize; i++)
            {
                _cartPiles.Add(new CardPile(EnumCardPile.Cart, i));
            }
        }

        private int GetCartBonus()
        {
            return _cartBonus;
        }

        private CartEvaluation GetCartEvaluation()
        {
            float bestValue = (float)BillServices.GetAmountDueToday() / ClientServices.NumClientsToday();

            var totalValue = _cartValue + _cartBonus;
            float percValue = totalValue / bestValue;
            return _cartEvaluations
                 .Where(e => e.Value > percValue)
                 .OrderBy(e => e.Value)
                 .FirstOrDefault();

        }

        private void RandomizeCart()
        {
            List<Card> allCards = new();
            foreach (var pile in _cartPiles)
            {
                foreach (var card in pile.Cards)
                {
                    allCards.Add(card);
                }
                pile.Clear();
            }
            allCards.Shuffle();

            int numPile = _cartPiles.Where(p => p.Enabled).Count();
            foreach (var card in allCards)
            {
                int index = UnityEngine.Random.Range(0, numPile);
                _cartPiles[index].AddOnTop(card);
                DeckEvents.OnPileUpdated(EnumCardPile.Cart, index);
            }

            UpdateUI();
            UpdateEffects();

        }

        private void Validate()
        {
            CartEvents.OnCartValidated(_cartPiles, _cartValue);
        }

        private Card GetTopCard()
        {
            if (_lastCartPile == null || _lastCartPile.IsEmpty)
            {
                Debug.LogWarning("No last cart pile found. Cannot get top card.");
                return null;
            }

            return _lastCartPile.TopCard;
        }



        private CardPile GetSelectedPile()
        {
            return _hoveredPile;
        }

        private void HoverPile(int index)
        {
            if (_cardBeingPlayed == null) return;

            _hoveredPile = _cartPiles.Find(p => p.Index == index);
            CartEvents.OnStackHovered(_cardBeingPlayed, _hoveredPile);


        }
        private void UnhoverPile(int index)
        {
            if (_hoveredPile != null && _hoveredPile.Index == index)
                _hoveredPile = null;
            CartEvents.OnStackHovered.Invoke(_cardBeingPlayed, null);
        }

        private void SetStacksHighlights(Card card)
        {
            _cardBeingPlayed = card;
            _hoveredPile = null;
            if (card == null)
            {
                CartEvents.OnStacksHighlighted(null);
                CartEvents.OnStackHovered(null, null);
                return;
            }
            if (!card.HasCartTarget) return;
            List<CardPile> compatiblePiles = CompatibleCardPile(card);
            CartEvents.OnStacksHighlighted(compatiblePiles.ConvertAll(p => p.Index).ToList());

        }

        private void ReplaceTopCard(Card card)
        {
            if (_lastCartPile == null || _lastCartPile.IsEmpty)
            {
                Debug.LogWarning("No last cart pile found. Cannot replace top card.");
                return;
            }

            DeckServices.MoveToTable(_lastCartPile.TopCard);

            _lastCartPile.AddOnTop(card);
            DeckEvents.OnPileUpdated(_lastCartPile.Type, _lastCartPile.Index);
            UpdateEffects();
            UpdateUI();
        }

        private void RemoveCard(Card card)
        {

            foreach (var pile in _cartPiles)
                pile.RemoveCard(card);
            UpdateEffects();
            UpdateUI();
        }

        private void ChangeCardCategory(EnumItemCategory category)
        {
            foreach (var pile in _cartPiles)
            {
                if (pile.IsEmpty || !pile.Enabled) continue;
                pile.OverrideStackCategory(category);
                DeckEvents.OnPileUpdated(pile.Type, pile.Index);
            }
            UpdateEffects();
            UpdateUI();
        }

        private bool AddCardToCart(Card card)
        {
            if (ActivatePreviousCard(card) ||
                MergeWithPrevious(card) ||
                AppendPileToCart(card))
            {

                UpdateUI();
                UpdateEffects();
                return true;
            }

            return false;
        }
        private void UpdateCardUI(Effect effect) => UpdateUI();
        private void UpdateUI()
        {
            UpdateCartSize();
            _cartPiles.ForEach(p => p.UpdateUI());
        }

        private void RestoreCategories()
        {

            foreach (var pile in _cartPiles)
            {
                if (pile.IsEmpty || !pile.Enabled) continue;
                pile.RestoreCategory();
            }
            UpdateEffects();
            UpdateUI();
        }

        private void SetRatingModifier(int value)
        {
            _ratingCartSizeModifier = value;
            UpdateCartSize();
        }

        private IEnumerator CalculateCart()
        {
            _cartBonus = 0;
            CartEvents.OnCartValueChanged?.Invoke();
            yield return new WaitForSeconds(0.4f);
            foreach (var cartPile in _cartPiles)
            {
                if (cartPile.IsEmpty || !cartPile.Enabled) continue;
                var rewards = GetPileRewards(cartPile.Index);
                foreach (var reward in rewards)
                {
                    _cartBonus += reward.Value;
                    if (reward.Value <= 0) continue;
                    CartEvents.OnCartRewardCalculated(cartPile.Index, reward, 0.8f);
                    yield return new WaitForSeconds(0.8f);
                    CartEvents.OnCartValueChanged?.Invoke();
                }
            }
        }

        private bool ActivatePreviousCard(Card pile)
        {
            return false;
            // if (_lastCartPile != null && !_lastCartPile.IsEmpty &&
            //     // _lastCartPile.TopCard.HasActivatableEffects &&
            //     _lastCartPile.Category == pile.TopCard.Category)
            // {
            //     // If the last cart pile is not empty and has the same category, merge into it
            //     _lastCartPile.MergeBelow(pile);
            //     DeckEvents.OnPileMoved(_lastCartPile.Type, _lastCartPile.Index);
            //     DeckEvents.OnCalculatingCartPile(_lastCartPile.Type, _lastCartPile.Index);
            //     _lastCartPile.TopCard.ActivatePower(_lastCartPile);
            //     return true; // Exit after merging to the last cart pile
            // }

            //  return false;
        }
        private List<EnumItemCategory> GetCategoriesInCart()
        {
            var categories = new List<EnumItemCategory>();
            foreach (var pile in _cartPiles)
            {
                if (!pile.Enabled || pile.IsEmpty) continue;
                foreach (var card in pile.Cards)
                {
                    if (card.Category != EnumItemCategory.Any)
                        categories.AddUnique(card.Category);
                }
            }
            return categories.ToList();
        }

        private int GetNumCardInCart(EnumItemCategory category)
        {
            int numCards = 0;
            foreach (var pile in _cartPiles)
            {
                if (!pile.Enabled || pile.IsEmpty) continue;
                numCards += pile.Cards.Count(card => card.Category == category || category == EnumItemCategory.Any);
            }
            return numCards;
        }

        private void UpdateCartSize()
        {
            int newCartSize = _initialCartSize + _ratingCartSizeModifier + EffectServices.GetCartSizeModifier();
            newCartSize = Mathf.Max(newCartSize, 2);

            while (_cartPiles.Count < newCartSize)
            {
                _cartPiles.Add(new CardPile(EnumCardPile.Cart, _cartPiles.Count));
            }


            for (int i = 0; i < _cartPiles.Count; i++)
            {
                if (CartSize >= newCartSize) break;
                var pile = _cartPiles[i];
                if (!pile.Enabled)
                    pile.Enabled = true;
            }

            //if the cart is too big, first try to disable empty piles starting from the right.
            for (int i = _cartPiles.Count - 1; i >= 0; i--)
            {
                if (CartSize <= newCartSize) break;
                var pile = _cartPiles[i];
                if (pile.IsEmpty && pile.Enabled)
                {
                    pile.Enabled = false;
                }
            }

            //if it is still too big, disable the last piles until it fits and discard the cards. 
            if (CartSize > newCartSize)
            {
                for (int i = _cartPiles.Count - 1; i >= 0; i--)
                {
                    if (_cartPiles[i].Enabled && !_cartPiles[i].IsEmpty)
                    {
                        DeckServices.Discard(new(_cartPiles[i].Cards));
                        DeckEvents.OnPileUpdated(_cartPiles[i].Type, _cartPiles[i].Index);
                        _cartPiles[i].Enabled = false;
                    }
                    if (CartSize <= newCartSize) break;
                }
            }

            DeckEvents.OnCardPoolSizeUpdate(EnumCardPile.Cart);

        }

        private void MergeCart(int amount, EnumItemCategory category)
        {
            // int index = _lastCartPile != null ? _cartPiles.IndexOf(_lastCartPile) : -1;
            //if (index == -1 || index == 0) return; // No other pile to merge into

            if (category == EnumItemCategory.Any)
            {
                while (CartPilesHaveSameCategory(out List<CardPile> piles))
                {
                    for (int i = piles.Count - 2; i >= 0; i--)
                    {
                        DeckController.MovePileTo(piles[i], piles[^1]);
                    }
                }
            }
            else
            {
                CardPile firstPile = null;
                List<CardPile> pilesToMerge = new();
                for (int i = _cartPiles.Count - 1; i >= 0; i--)
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
                    DeckController.MovePileTo(pile, firstPile);

                }
            }

            // _lastCartPile = _cartPiles[index - 1];
            UpdateUI();
            UpdateEffects();
            // DeckEvents.OnCalculatingCartPile(_lastCartPile.Type, _lastCartPile.Index);
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

        private List<CardReward> GetPileRewards(int index)
        {
            var pile = _cartPiles.Find(p => p.Index == index);

            List<CardPile> otherCartPiles =
                _cartPiles.Where((p, i) => i != index).ToList();

            return pile.CalculateCartRewards(otherCartPiles);
        }
        private void UpdateEffects()
        {
            EffectServices.UpdateCardEffects(
                      _cartPiles.FindAll(p => p.Enabled && !p.IsEmpty)
                              .ConvertAll(p => p.TopCard));
        }
        private bool AddCardToCartPile(Card topCard, CardPile pile)
        {
            var mergeEffect = topCard.Effects.Find(effect => effect.Data is MergeWithPileEffect);
            if (mergeEffect == null)
            {

                pile.AddOnTop(topCard);
                _lastCartPile = pile;
                UpdateEffects();
                UpdateUI();
                CartEvents.OnNewCartPileUsed(topCard);
                return true; // If no effect, just add to the pile
            }

            var effectData = mergeEffect.Data as MergeWithPileEffect;

            if (effectData.Location == EnumPlacement.AtTheBottom)
                pile.AddAtTheBottom(topCard);
            else
                pile.AddOnTop(topCard);
            _lastCartPile = pile;
            UpdateEffects();
            UpdateUI();
            return true; // Exit after adding to the specified pile
        }

        private bool MergeWithPrevious(Card topCard)
        {

            var effect = topCard.Effects.Find(effect => effect.Data is MergeWithPileEffect);
            if (effect == null) return false;

            var effectData = effect.Data as MergeWithPileEffect;
            CardPile cardPileRef = null;

            // if (effectData.TargetStack == MergeWithPileEffect.EnumTargetStack.Previous)
            // {
            //     if (_lastCartPile == null || _lastCartPile.IsEmpty)
            //     {
            //         if (!effectData.AllowEmptyPiles)
            //             Debug.LogWarning(
            //                 "MergeWithPreviousPileEffect: No last cart pile to merge with. This card should not be playable");
            //         return false;
            //     }
            //     cardPileRef = _lastCartPile;
            // }
            if (effectData.TargetStack == MergeWithPileEffect.EnumTargetStack.LowestStack)
            {
                cardPileRef = _cartPiles
                    .Where(p => p.Enabled && !p.IsEmpty)
                    .OrderBy(p => p.TopCard.Price)
                    .FirstOrDefault();
            }
            // else if (effectData.TargetStack == MergeWithPileEffect.EnumTargetStack.SameCategory)
            // {
            //     cardPileRef = _cartPiles
            //         .Where(p => p.Enabled && !p.IsEmpty && p.Category == topCard.Category)
            //         .FirstOrDefault();
            // }
            if (cardPileRef == null || cardPileRef.IsEmpty)
            {
                if (!effectData.AllowEmptyPiles)
                    Debug.LogWarning("MergeWithPreviousPileEffect: No valid previous pile found. This card should not have been playable");
                return false;
            }

            if (effectData.Category != EnumItemCategory.Any &&
               effectData.Category != cardPileRef.Category)
            {
                Debug.LogWarning("CAtegory is not matching: No valid previous pile found. This card should not have been playable");
                return false;
            }

            if (effectData.Location == EnumPlacement.AtTheBottom)
            {
                cardPileRef.AddAtTheBottom(topCard);

                return true;
            }
            else if (effectData.Location == EnumPlacement.OnTop)
            {
                cardPileRef.AddOnTop(topCard);

                return true; // Exit after merging to the last cart pile
            }
            else
            {
                Debug.LogWarning($"MergeWithPreviousPileEffect: Unknown location {effectData.Location} for card {topCard.Item.Data.name}");
            }

            return false;
        }
        private bool AppendPileToCart(Card card)
        {
            var cartPile = _cartPiles.Find(c => c.IsEmpty && c.Enabled);
            if (cartPile == null) return false;


            cartPile.AddOnTop(card);
            _lastCartPile = cartPile;
            return true;
        }

        private void DiscardCart()
        {
            EffectServices.RemoveEffectsLinkedToPiles(_cartPiles);
            List<Card> cardsToDiscard = new();
            foreach (var cartPile in _cartPiles)
                foreach (var card in cartPile.Cards)
                    cardsToDiscard.Add(card);

            DeckServices.Discard(cardsToDiscard);
            CartEvents.OnCartCleared?.Invoke();

            _lastCartPile = null;
            UpdateEffects();
            UpdateUI();
        }

        private bool CanAddToCart(Card topCard)
        {
            return CompatibleCardPile(topCard).Count > 0;
        }


        private List<CardPile> CompatibleCardPile(Card card)
        {

            var emptyPiles = _cartPiles.FindAll(p => p.Enabled && p.IsEmpty);
            var mergeEffects = card.Effects.FindAll(effect => effect.Data is MergeWithPileEffect);
            if (mergeEffects.Count == 0)
                return emptyPiles;

            if (mergeEffects.Count > 1)
            {
                Debug.LogWarning($"Multiple merge effects found for card {card.Item.Data.name}. Only the first one will be used.");
            }

            List<CardPile> compatiblePiles = new();

            var mergeEffect = mergeEffects[0];
            var mergeEffectData = mergeEffect.Data as MergeWithPileEffect;

            if (mergeEffectData.AllowEmptyPiles)
                compatiblePiles.AddRange(emptyPiles);


            if (mergeEffectData.Location == EnumPlacement.OnTop)
                compatiblePiles.AddRange(_cartPiles.FindAll(p =>
                            p.Enabled &&
                             !p.IsEmpty &&
                             !p.TopCard.CannotBeCovered &&
                             (mergeEffectData.Category == EnumItemCategory.Any ||
                                p.Category == mergeEffectData.Category)));

            if (mergeEffectData.Location == EnumPlacement.AtTheBottom)
                compatiblePiles.AddRange(_cartPiles.FindAll(p =>
                                p.Enabled &&
                                 !p.IsEmpty &&
                                 (mergeEffectData.Category == EnumItemCategory.Any ||
                                    p.Category == mergeEffectData.Category)));

            if (compatiblePiles.Count == 0) return compatiblePiles;

            if (mergeEffectData.TargetStack == MergeWithPileEffect.EnumTargetStack.LowestStack)
            {
                int lowestStack = compatiblePiles.Where(p => p.Enabled && !p.IsEmpty)
                    .OrderBy(p => p.Count)
                    .FirstOrDefault()?.Count ?? 0;

                return compatiblePiles.FindAll(p => p.Enabled && !p.IsEmpty && p.Count == lowestStack);
            }
            if (mergeEffectData.TargetStack == MergeWithPileEffect.EnumTargetStack.HighestStack)
            {
                var highestStack = compatiblePiles.Where(p => p.Enabled && !p.IsEmpty)
                    .OrderBy(p => p.Count)
                    .LastOrDefault()?.Count ?? 0;
                return compatiblePiles.FindAll(p => p.Enabled && !p.IsEmpty && p.Count == highestStack);
            }

            if (mergeEffectData.TargetStack == MergeWithPileEffect.EnumTargetStack.LowestValue)
            {
                var lowestValue = compatiblePiles.Where(p => p.Enabled && !p.IsEmpty)
                    .OrderBy(p => p.TopCard.Price)
                    .LastOrDefault()?.TopCard.Price ?? 0;
                return compatiblePiles.FindAll(p => p.Enabled && !p.IsEmpty && p.TopCard.Price == lowestValue);
            }
            if (mergeEffectData.TargetStack == MergeWithPileEffect.EnumTargetStack.HighestValue)
            {
                var highestValue = compatiblePiles.Where(p => p.Enabled && !p.IsEmpty)
                    .OrderBy(p => p.TopCard.Price)
                    .FirstOrDefault()?.TopCard.Price ?? 0;
                return compatiblePiles.FindAll(p => p.Enabled && !p.IsEmpty && p.TopCard.Price == highestValue);
            }


            return compatiblePiles;
        }


    }
}
