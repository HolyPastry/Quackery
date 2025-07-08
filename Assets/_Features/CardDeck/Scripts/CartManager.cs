using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Holypastry.Bakery;
using Quackery.Effects;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.Decks
{

    public class CartManager : MonoBehaviour
    {

        [SerializeField] private int _initialCartSize = 3;

        private int _ratingCartSizeModifier;
        // private int _cardCartSizeModifier;
        private bool _cartCalculated;

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
                CartEvents.OnCartValueChanged?.Invoke(_cartValue);
            }
        }

        private CardPile _lastCartPile;
        private CardPile _selectedPile;

        void OnEnable()
        {

            CartServices.SetRatingCartSizeModifier = SetRatingModifier;
            // CartServices.ModifyCardCartSizeModifier = ModifyCardModifier;
            // CartServices.ResetCartSizeCardModifier = ResetCartSizeCardModifier;
            CartServices.GetCartSize = () => CartSize;

            CartServices.DiscardCart = DiscardCart;
            CartServices.MergeCart = MergeCart;

            CartServices.GetNumCardInCart = GetNumCardInCart;
            CartServices.GetCategoriesInCart = GetCategoriesInCart;
            CartServices.CalculateCart = CalculateCart;
            CartServices.CompleteCartPileCalculation = () => _cartCalculated = true;

            CartServices.AddCardToCart = AddCardToCart;
            CartServices.CanAddToCart = CanAddToCart;

            CartServices.RestoreCategories = RestoreCategories;
            CartServices.ChangeCardCategory = ChangeCardCategory;
            CartServices.RemoveCard = RemoveCard;
            CartServices.GetPileRewards = GetPileRewards;

            CartServices.AddToCartValue = value => CartValue += value;
            CartServices.SetCartValue = value => CartValue = value;
            CartServices.GetCartValue = () => CartValue;
            CartServices.CanCartAfford = value => CartValue >= value;

            CartServices.ReplaceTopCard = ReplaceTopCard;
            CartServices.GetTopCard = GetTopCard;

            CartServices.ValidateCart = Validate;

            CartServices.SetStacksHighlights = SetStacksHighlights;
            CartServices.GetSelectedPile = GetSelectedPile;
            CartServices.SelectPile = SelectPile;
            CartServices.DeselectPile = DeselectPile;
            CartServices.AddCardToCartPile = AddCardToCartPile;


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
            CartServices.CompleteCartPileCalculation = delegate { };

            CartServices.AddCardToCart = (pile) => false;
            CartServices.CanAddToCart = (card) => true;

            CartServices.RestoreCategories = delegate { };
            CartServices.ChangeCardCategory = delegate { };

            CartServices.RemoveCard = delegate { };
            CartServices.GetPileRewards = (index) => new();

            CartServices.AddToCartValue = value => { };
            CartServices.SetCartValue = value => { };
            CartServices.GetCartValue = () => 0;
            CartServices.CanCartAfford = value => true;

            CartServices.ReplaceTopCard = delegate { };
            CartServices.GetTopCard = () => null;

            CartServices.ValidateCart = () => { };

            CartServices.SetStacksHighlights = delegate { };
            CartServices.GetSelectedPile = () => null;
            CartServices.SelectPile = (index) => { };
            CartServices.DeselectPile = (index) => { };
            // CartServices.AddCardToCartPile = (card, pile) => true;
            CartServices.AddCardToCartPile = (card, pile) => false;

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
            return _selectedPile;
        }

        private void SelectPile(int index)
        {
            _selectedPile = _cartPiles.Find(p => p.Index == index);
        }
        private void DeselectPile(int index)
        {
            if (_selectedPile != null && _selectedPile.Index == index)
                _selectedPile = null;
        }

        private void SetStacksHighlights(Card card)
        {
            if (card == null)
            {
                CartEvents.OnStacksHighlighted(null);
                return;
            }
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

            _lastCartPile.AddAtTheTop(card);
            DeckEvents.OnPileUpdated(_lastCartPile.Type, _lastCartPile.Index);
        }

        private void RemoveCard(Card card)
        {

            foreach (var pile in _cartPiles)
                pile.RemoveCard(card);
        }

        private void ChangeCardCategory(EnumItemCategory category)
        {
            foreach (var pile in _cartPiles)
            {
                if (pile.IsEmpty || !pile.Enabled) continue;
                pile.OverrideStackCategory(category);
                DeckEvents.OnPileUpdated(pile.Type, pile.Index);
            }
        }

        private bool AddCardToCart(Card card)
        {
            if (ActivatePreviousCard(card) ||
                MergeWithPrevious(card) ||
                AppendPileToCart(card))
            {

                UpdateUI();
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
        }




        // private void ModifyCardModifier(int value)
        // {
        //     _cardCartSizeModifier += value;
        //     UpdateCartSize();
        // }
        // private void ResetCartSizeCardModifier()
        // {
        //     _cardCartSizeModifier = 0;
        //     UpdateCartSize();
        // }

        private void SetRatingModifier(int value)
        {
            _ratingCartSizeModifier = value;
            UpdateCartSize();
        }

        private IEnumerator CalculateCart()
        {
            if (_lastCartPile == null || !_lastCartPile.Enabled)
            {
                Debug.LogWarning("No last cart pile found. Cannot calculate cart.");
                yield break;
            }

            if (_lastCartPile.IsEmpty) yield break;
            EffectServices.ExecutePile(EnumEffectTrigger.BeforeCartCalculation, _lastCartPile);

            _cartCalculated = false;
            CartEvents.OnCalculatingCartPile(_lastCartPile.Index);
            yield return new WaitUntil(() => _cartCalculated);
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
                    if (card.Category != EnumItemCategory.Unset)
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
                numCards += pile.Cards.Count(card => card.Category == category || category == EnumItemCategory.Unset);
            }
            return numCards;
        }
        private void RecountCart()
        {
            foreach (var pile in _cartPiles)
            {
                if (pile.IsEmpty || !pile.Enabled) continue;
                //Cart.OnCalculatingCartPile(pile.Type, pile.Index);
            }
        }
        private void UpdateCartSize()
        {
            int cartSize = _initialCartSize + _ratingCartSizeModifier + EffectServices.GetCartSizeModifier();
            cartSize = Mathf.Max(cartSize, 2);

            while (_cartPiles.Count <= cartSize)
            {
                _cartPiles.Add(new CardPile(EnumCardPile.Cart, _cartPiles.Count));
            }
            for (int i = 0; i < _cartPiles.Count; i++)
            {
                _cartPiles[i].Enabled = i < cartSize;
            }
            DeckEvents.OnCardPoolSizeUpdate(EnumCardPile.Cart);
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
                        DeckController.MovePileTo(piles[i], piles[0]);
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
                    DeckController.MovePileTo(pile, firstPile);
                }
            }

            _lastCartPile = _cartPiles[index - 1];
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

        private bool AddCardToCartPile(Card topCard, CardPile pile)
        {
            var effect = topCard.Effects.Find(effect => effect.Data is MergeWithPileEffect);
            if (effect == null)
            {
                pile.AddAtTheTop(topCard);
                _lastCartPile = pile;
                UpdateUI();
                return true; // If no effect, just add to the pile
            }

            var effectData = effect.Data as MergeWithPileEffect;

            if (effectData.Location == EnumPileLocation.AtTheBottom)
                pile.AddAtTheBottom(topCard);
            else
                pile.AddAtTheTop(topCard);
            _lastCartPile = pile;
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

            if (effectData.Category != EnumItemCategory.Unset &&
               effectData.Category != cardPileRef.Category)
            {
                Debug.LogWarning("CAtegory is not matching: No valid previous pile found. This card should not have been playable");
                return false;
            }

            if (effectData.Location == EnumPileLocation.AtTheBottom)
            {
                cardPileRef.AddAtTheBottom(topCard);
                return true;
            }
            else if (effectData.Location == EnumPileLocation.OnTop)
            {
                cardPileRef.AddAtTheTop(topCard);
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


            cartPile.AddAtTheTop(card);
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

            _lastCartPile = null;
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


            if (mergeEffectData.Location == EnumPileLocation.OnTop)
                compatiblePiles.AddRange(_cartPiles.FindAll(p =>
                            p.Enabled &&
                             !p.IsEmpty &&
                             !p.TopCard.CannotBeCovered &&
                             (mergeEffectData.Category == EnumItemCategory.Unset ||
                                p.Category == mergeEffectData.Category)));

            if (mergeEffectData.Location == EnumPileLocation.AtTheBottom)
                compatiblePiles.AddRange(_cartPiles.FindAll(p =>
                                p.Enabled &&
                                 !p.IsEmpty &&
                                 (mergeEffectData.Category == EnumItemCategory.Unset ||
                                    p.Category == mergeEffectData.Category)));

            if (compatiblePiles.Count == 0) return compatiblePiles;

            if (mergeEffectData.TargetStack == MergeWithPileEffect.EnumTargetStack.LowestStack)
            {
                var lowestStack = compatiblePiles.FindAll(p => p.Enabled && !p.IsEmpty)
                    .OrderBy(p => p.Count)
                    .LastOrDefault();
                return new() { lowestStack };
            }
            if (mergeEffectData.TargetStack == MergeWithPileEffect.EnumTargetStack.HighestValue)
            {
                var lowestStack = compatiblePiles.FindAll(p => p.Enabled && !p.IsEmpty)
                    .OrderBy(p => p.Count)
                    .FirstOrDefault();
                return new() { lowestStack };
            }
            if (mergeEffectData.TargetStack == MergeWithPileEffect.EnumTargetStack.LowestValue)
            {
                var lowestValue = compatiblePiles.FindAll(p => p.Enabled && !p.IsEmpty)
                    .OrderBy(p => p.TopCard.Price)
                    .LastOrDefault();
                return new() { lowestValue };
            }
            if (mergeEffectData.TargetStack == MergeWithPileEffect.EnumTargetStack.HighestValue)
            {
                var lowestValue = compatiblePiles.FindAll(p => p.Enabled && !p.IsEmpty)
                    .OrderBy(p => p.TopCard.Price)
                    .FirstOrDefault();
                return new() { lowestValue };
            }


            return compatiblePiles;
        }


    }
}
