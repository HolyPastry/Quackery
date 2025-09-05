using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Holypastry.Bakery;

using Quackery.Clients;
using Quackery.Effects;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.Decks
{
    public class CartController : CardPilePool
    {

        [SerializeField]
        private List<CartEvaluation> _cartEvaluations = new();

        private int _cartBonus;
        private int _cartValue;

        private int _cartTotalValue;

        private readonly Dictionary<int, int> RewardHistory = new();

        public bool CartIsFull => _cardPiles.TrueForAll(p => !p.IsEmpty || !p.Enabled);

        public int CartSize => _cardPiles.Count(p => p.Enabled);

        public int CartValue
        {
            get => _cartValue;
            private set
            {
                _cartValue = value;
                CartEvents.OnValueChanged?.Invoke();
            }
        }

        public int CartBonus
        {
            get => _cartBonus;
            private set
            {
                var delta = _cartBonus - value;
                _cartBonus = value;
                CartEvents.OnBonusChanged?.Invoke(delta);
            }
        }

        public int TotalValue
        {
            get => _cartTotalValue;
            private set
            {
                _cartTotalValue = value;
                CartEvents.OnTotalValueChanged?.Invoke();
            }
        }

        private CardPile _lastCartPile;

        private Card _cardBeingPlayed;
        private CardPile _hoveredPile;
        private CartMode _cartMode = CartMode.Survival;


        private bool IsTopCard(Card card) => GetTopCards().Contains(card);

        void OnEnable()
        {

            CartServices.SetRatingCartSizeModifier = SetRatingModifier;
            CartServices.GetCartSize = () => CartSize;

            CartServices.ResetCart = ResetCart;
            CartServices.InitCart = InitCart;

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

            CartServices.CalculateCart = () => StartCoroutine(CalculateCart());
            CartServices.GetBonus = () => CartBonus;


            CartServices.AddToCartValue = value => CartValue += value;

            CartServices.GetValue = () => CartValue;
            CartServices.GetTotalValue = () => _cartTotalValue;
            CartServices.CanCartAfford = value => CartValue + CartBonus >= value;


            CartServices.ReplaceTopCard = ReplaceTopCard;
            CartServices.GetTopCard = GetTopCard;

            CartServices.ValidateCart = Validate;

            CartServices.SetStacksHighlights = SetStacksHighlights;
            CartServices.GetHoveredPile = GetSelectedPile;
            CartServices.HoverPile = HoverPile;
            CartServices.UnhoverPile = UnhoverPile;

            CartServices.IsTopCard = IsTopCard;


            CartServices.RandomizeCart = RandomizeCart;
            //            CartServices.GetCartEvaluation = GetCartEvaluation;

            CartServices.GetMaxValue = GetMaxValue;
            CartServices.GetMode = () => _cartMode;
            CartServices.GetMatchingCards = GetMatchingCards;


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
            CartServices.ResetCart = () => { };
            CartServices.GetValue = () => 0;
            CartServices.CanCartAfford = value => false;

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
            // CartServices.GetCartEvaluation = () => default;
            CartServices.IsTopCard = (topCard) => false;

            CartServices.GetMaxValue = () => 30;
            CartServices.GetMode = () => CartMode.Survival;
            CartServices.GetMatchingCards = (predicate) => new List<Card>();

            EffectEvents.OnAdded -= UpdateCardUI;
            EffectEvents.OnRemoved -= UpdateCardUI;
            EffectEvents.OnUpdated -= UpdateCardUI;
        }

        private List<Card> GetMatchingCards(Predicate<Card> predicate)
        {

            return _cardPiles
                .Where(p => p.Enabled && !p.IsEmpty)
                .SelectMany(p => p.Cards)
                .Where(c => predicate(c))
                .ToList();
        }


        private void InitCart()
        {
            UpdateUI();
        }
        private void ResetCart()
        {
            CartBonus = 1;
            CartValue = 0;
            TotalValue = 0;
            RewardHistory.Clear();
            UpdateCartMode();
            DiscardCart();
        }

        private int GetMaxValue()
        {
            if (_cartMode == CartMode.SuperSaiyan) return -1;
            return ClientServices.GetThreshold(_cartMode);
        }



        // private CartEvaluation GetCartEvaluation()
        // {
        //     float bestValue = (float)BillServices.GetAmountDueToday() / ClientServices.NumClientsToday();

        //     var totalValue = _cartValue + _cartBonus;
        //     float percValue = totalValue / bestValue;

        //     if (percValue <= 0) return _cartEvaluations[^1];

        //     if (percValue >= 1) return _cartEvaluations[0];

        //     for (int i = 0; i < _cartEvaluations.Count - 1; i++)
        //     {
        //         float threshold = _cartEvaluations[i].Value;
        //         if (percValue >= threshold)
        //         {
        //             return _cartEvaluations[i];
        //         }
        //     }
        //     Debug.LogWarning($"No cart evaluation found for value {percValue}. Returning default.");
        //     return _cartEvaluations[^1];

        // }

        private void RandomizeCart()
        {
            List<Card> allCards = new();
            foreach (var pile in _cardPiles)
                allCards.AddRange(pile.RemoveAllCards());

            allCards.Shuffle();

            int numPile = _cardPiles.Where(p => p.Enabled).Count();
            foreach (var card in allCards)
            {
                int index = UnityEngine.Random.Range(0, numPile);
                _cardPiles[index].AddOnTop(card);
                DeckEvents.OnPileUpdated(EnumCardPile.Cart, index);
            }

            UpdateUI();
            UpdateEffects();

        }

        private void Validate()
        {
            CartEvents.OnCartValidated(_cardPiles, _cartValue);
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

        private void HoverPile(CardPile hoveredPile)
        {
            if (_cardBeingPlayed == null) return;
            _hoveredPile = hoveredPile;
            CartEvents.OnStackHovered(_cardBeingPlayed, hoveredPile);


        }
        private void UnhoverPile(CardPile hoveredPile)
        {
            if (_hoveredPile != null && _hoveredPile == hoveredPile)
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

            List<CardPile> compatiblePiles = CompatibleCardPile(card);
            CartEvents.OnStacksHighlighted(compatiblePiles);

        }

        private void ReplaceTopCard(Card card)
        {
            if (_lastCartPile == null || _lastCartPile.IsEmpty)
            {
                Debug.LogWarning("No last cart pile found. Cannot replace top card.");
                return;
            }

            DeckServices.MoveToHand(_lastCartPile.TopCard);

            _lastCartPile.AddOnTop(card);
            UpdateEffects();
            UpdateUI();
        }

        private void RemoveCard(Card card, bool removeValue)
        {
            if (removeValue)
                CartValue -= card.Price;
            foreach (var pile in _cardPiles)
            {
                if (!pile.RemoveCard(card)) continue;
                UpdateEffects();
                UpdateUI();
            }

        }

        private void ChangeCardCategory(EnumItemCategory category)
        {
            foreach (var pile in _cardPiles)
            {
                if (pile.IsEmpty || !pile.Enabled) continue;
                pile.OverrideStackCategory(category);
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
            _cardPiles.ForEach(p => p.UpdateUI());
        }

        private void RestoreCategories()
        {

            foreach (var pile in _cardPiles)
            {
                if (pile.IsEmpty || !pile.Enabled) continue;
                pile.RestoreCategory();
            }
            UpdateEffects();
            UpdateUI();
        }

        private void SetRatingModifier(int value)
        {

            UpdateCartSize();
        }

        private IEnumerator CalculateCart()
        {

            // CartEvents.OnValueChanged?.Invoke();

            CartBonus = 1;
            CartValue = 0;
            yield return Tempo.WaitForABeat;
            foreach (var cartPile in _cardPiles)
            {
                if (cartPile.IsEmpty || !cartPile.Enabled) continue;
                int pileIndex = _cardPiles.IndexOf(cartPile);
                var rewards = GetPileRewards(cartPile as CartPile);
                foreach (var reward in rewards)
                    yield return StartCoroutine(GiveReward(reward, cartPile as CartPile));
                // CardReward reward;
                // if (rewards.Count > 0)
                //     reward = rewards[0];
                // else
                //     reward = new CardReward()
                //     {
                //         Type = EnumCardReward.Synergy,
                //         Value = 0
                //     };


                // int deltaScore = 0;

                // if (RewardHistory.ContainsKey(pileIndex))
                // {
                //     deltaScore = reward.Value - RewardHistory[pileIndex];
                //     RewardHistory[pileIndex] = reward.Value;
                // }
                // else
                // {
                //     RewardHistory.Add(pileIndex, reward.Value);
                //     deltaScore = reward.Value;
                // }

                // _cartBonus += deltaScore;
                // CartEvents.OnCartRewardCalculated(pileIndex, reward, deltaScore, 0.5f);
                // if (deltaScore != 0)
                // {
                //     yield return Tempo.WaitForABeat;
                //     CartEvents.OnBonusChanged(deltaScore);
                // }
            }
            yield return Tempo.WaitForHalfABeat;
            TotalValue += CartValue * CartBonus;
            CartValue = 0;
            CartBonus = 1;

            UpdateCartMode();
            CartEvents.OnCalculationCompleted();
            yield return Tempo.WaitForABeat;
        }

        private IEnumerator GiveReward(CardReward reward, CartPile cartPile)
        {
            yield return cartPile.ShowRewardLabel(reward);
            if (reward.Multiplier > 0)
            {
                CartBonus += reward.Multiplier;
                yield return cartPile.PunchRewardNumber($"x{reward.Multiplier}");

            }
            if (reward.Value > 0)
            {
                CartValue += reward.Value;
                yield return cartPile.PunchRewardNumber($"+{reward.Value}");
            }
            cartPile.HideRewardLabel();

        }

        private void UpdateCartMode()
        {
            if (CartValue + CartBonus < ClientServices.GetThreshold(CartMode.Survival))
            {
                if (_cartMode != CartMode.Survival)
                {

                    _cartMode = CartMode.Survival;
                    StartCoroutine(ShowCelebrationUI(CartMode.Survival));
                }

            }
            else if (CartValue + CartBonus < ClientServices.GetThreshold(CartMode.Normal))
            {
                if (_cartMode != CartMode.Normal)
                {
                    _cartMode = CartMode.Normal;
                    StartCoroutine(ShowCelebrationUI(CartMode.Normal));
                }

            }
            else
            {
                if (_cartMode != CartMode.SuperSaiyan)
                {
                    _cartMode = CartMode.SuperSaiyan;
                    StartCoroutine(ShowCelebrationUI(CartMode.SuperSaiyan));
                }

            }
            CartEvents.OnModeChanged(_cartMode);
        }

        private IEnumerator ShowCelebrationUI(CartMode survival)
        {
            if (!_cartEvaluations.Exists(e => e.Mode == survival)) yield break;


            var evaluation = _cartEvaluations.Find(e => e.Mode == survival);
            evaluation.RealizationObjectReference.SetActive(true);
            yield return new WaitForSeconds(evaluation.Duration);
            evaluation.RealizationObjectReference.SetActive(false);

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
            foreach (var pile in _cardPiles)
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
            foreach (var pile in _cardPiles)
            {
                if (!pile.Enabled || pile.IsEmpty) continue;
                numCards += pile.Cards.Count(card => card.Category == category || category == EnumItemCategory.Any);
            }
            return numCards;
        }

        private void UpdateCartSize()
        {
            int newCartSize = ClientServices.GetCartSize() +
                (int)EffectServices.GetModifier(typeof(CartSizeModifierEffect));

            newCartSize = Mathf.Max(newCartSize, 2);
            //if (CartSize == newCartSize) return;

            StartCoroutine(SetPoolSize(newCartSize));


        }

        private void MergeCart(int amount, EnumItemCategory category)
        {
            // int index = _lastCartPile != null ? _cardPiles.IndexOf(_lastCartPile) : -1;
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
                for (int i = _cardPiles.Count - 1; i >= 0; i--)
                {
                    if (_cardPiles[i].IsEmpty ||
                        !_cardPiles[i].Enabled ||
                        _cardPiles[i].Category != category) continue;

                    if (firstPile == null)
                        firstPile = _cardPiles[i];
                    else
                        pilesToMerge.Add(_cardPiles[i]);
                }
                foreach (var pile in pilesToMerge)
                {
                    DeckController.MovePileTo(pile, firstPile);

                }
            }

            // _lastCartPile = _cardPiles[index - 1];
            UpdateUI();
            UpdateEffects();
            // DeckEvents.OnCalculatingCartPile(_lastCartPile.Type, _lastCartPile.Index);
        }

        private bool CartPilesHaveSameCategory(out List<CardPile> piles)
        {
            //piles = _cardPiles.FindAll(p => p.Enabled && !p.IsEmpty);
            piles = new List<CardPile>();
            for (int i = 0; i < _cardPiles.Count; i++)
            {
                if (!_cardPiles[i].Enabled || _cardPiles[i].IsEmpty) continue;
                for (int j = i + 1; j < _cardPiles.Count; j++)
                {
                    if (!_cardPiles[j].Enabled || _cardPiles[j].IsEmpty) continue;
                    if (_cardPiles[i].Category != _cardPiles[j].Category) continue;
                    piles.AddUnique(_cardPiles[i]);
                    piles.AddUnique(_cardPiles[j]);
                    return true; // Found at least two piles with the same category
                }
            }
            return piles.Count >= 2;
        }

        private List<CardReward> GetPileRewards(CartPile cardPile)
        {
            var topCard = cardPile.TopCard;
            var subItems = cardPile.Cards.Where(c => c != topCard).Select(c => c.Item).ToList();
            List<CardReward> rewards = new();

            (int multiplier, int bonusValue) = CardEffectServices.GetSynergyBonuses(topCard, subItems);
            rewards.Add(new()
            {
                Type = EnumCardReward.BaseReward,
                Value = topCard.Price
            });

            if (subItems.Count > 0 && subItems.TrueForAll(i => i.Category == topCard.Category))
            {
                rewards.Add(new()
                {
                    Type = EnumCardReward.Synergy,
                    Multiplier = multiplier,
                    ShowLabel = true
                });
            }

            int numMatching = GetNumMatchingNeighbors(cardPile);

            if (numMatching > 0)
            {
                EnumCardReward type = numMatching switch
                {
                    1 => EnumCardReward.Pair,
                    2 => EnumCardReward.ThreeOfAKind,
                    3 => EnumCardReward.FourOfAKind,
                    _ => EnumCardReward.FourOfAKind,
                };
                rewards.Add(new()
                {
                    Type = type,
                    Multiplier = Mathf.Min(4, numMatching + 1),
                    ShowLabel = true,
                });
            }



            return rewards;
        }

        private int GetNumMatchingNeighbors(CardPile cardPile)
        {
            var price = cardPile.TopCard.Price;
            var enabledPiles = EnabledPiles.ToList();
            int index = enabledPiles.IndexOf(cardPile);

            int numMatchingNeightbor = 0;
            int range = 1;
            while (index + range < enabledPiles.Count &&
                    !enabledPiles[index + range].IsEmpty &&
                    enabledPiles[index + range].Enabled &&
                    enabledPiles[index + range].TopCard.Price == price
                    )
            {
                numMatchingNeightbor++;
                range++;
            }
            range = -1;
            while (index + range >= 0 &&
                   !enabledPiles[index + range].IsEmpty &&
                   enabledPiles[index + range].Enabled &&
                   enabledPiles[index + range].TopCard.Price == price
                   )
            {
                numMatchingNeightbor++;
                range--;
            }

            return numMatchingNeightbor;
        }

        private void UpdateEffects()
        {
            var topCards = _cardPiles.FindAll(p => p.Enabled && !p.IsEmpty)
                              .ConvertAll(p => p.TopCard);
            // EffectServices.Remove(e => e.LinkedObject is Card card &&
            //     !topCards.Exists(c => c == card));
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


            if (effectData.TargetStack == MergeWithPileEffect.EnumTargetStack.LowestStack)
            {
                cardPileRef = _cardPiles
                    .Where(p => p.Enabled && !p.IsEmpty)
                    .OrderBy(p => p.TopCard.Price)
                    .FirstOrDefault();
            }

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
            var cartPile = _cardPiles.Find(c => c.IsEmpty && c.Enabled);
            if (cartPile == null) return false;


            cartPile.AddOnTop(card);
            _lastCartPile = cartPile;
            return true;
        }

        private void DiscardCart()
        {
            List<Card> topCards = GetTopCards();
            EffectServices.Remove(e => topCards.Exists(card => (object)card.gameObject == e.LinkedObject));

            List<Card> cardsToDiscard = new();
            foreach (var cartPile in _cardPiles)
                foreach (var card in cartPile.Cards)
                    cardsToDiscard.Add(card);

            DeckServices.Discard(cardsToDiscard);
            CartEvents.OnCartCleared?.Invoke();

            _lastCartPile = null;
            UpdateEffects();
            UpdateUI();
        }

        private List<Card> GetTopCards()
        {
            return _cardPiles.Where(p => !p.IsEmpty)
                        .Select(p => p.TopCard).ToList();
        }

        private bool CanAddToCart(Card topCard)
        {
            return CompatibleCardPile(topCard).Count > 0;
        }


        private List<CardPile> CompatibleCardPile(Card card)
        {
            if (!card.HasCartTarget) return new();
            // return _cardPiles.FindAll(p => p.Enabled);

            var emptyPiles = _cardPiles.FindAll(p => p.Enabled && p.IsEmpty);
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
                compatiblePiles.AddRange(_cardPiles.FindAll(p =>
                            p.Enabled &&
                             !p.IsEmpty &&
                             !p.TopCard.CannotBeCovered &&
                             (mergeEffectData.Category == EnumItemCategory.Any ||
                                p.Category == mergeEffectData.Category)));

            if (mergeEffectData.Location == EnumPlacement.AtTheBottom)
                compatiblePiles.AddRange(_cardPiles.FindAll(p =>
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
