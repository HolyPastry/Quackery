using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Holypastry.Bakery;

using Quackery.Artifacts;
using Quackery.Decks;
using Quackery.Inventories;

using UnityEngine;
using UnityEngine.Assertions;

namespace Quackery.Effects
{
    public class EffectManager : MonoBehaviour
    {
        private readonly List<Effect> _effects = new();
        private DataCollection<EffectData> _effectCollection;

        void Awake()
        {
            _effectCollection = new("Effects");
        }

        void OnDisable()
        {
            EffectServices.AddEffect = (effectData) => { };
            EffectServices.Cancel = delegate { };

            EffectServices.Execute = (trigger, card) => null;
            EffectServices.ExecutePile = (trigger, cardPile) => null;

            EffectServices.GetCurrent = () => new();

            EffectServices.ModifyValue = (effectData, value) => { };
            EffectServices.SetValue = (effectData, value) => { };
            EffectServices.GetValue = (effectData) => 1;

            EffectServices.IsCardPlayable = (card) => true;

            EffectServices.RemoveEffectsLinkedToPiles = delegate { };
            EffectServices.GetCardPrice = (card) => 0;
            EffectServices.GetStackPrice = (topCard, subItems) => 0;

            EffectServices.CleanEffects = delegate { _effects.Clear(); };

            EffectServices.CancelAllEffects = delegate { };
            EffectServices.ChangePreference = (category) => { };
            EffectServices.CounterEffect = (itemData, numCard) => 0;
            EffectServices.GetCartSizeModifier = () => 0;
            EffectServices.RemoveArtifactEffects = (artifactData) => { };

            EffectServices.GetSynergyBonuses = (card, subItems) => (0, 0);
            EffectServices.UpdateCardEffects = (topCards) => { };

            EffectServices.AddEffectsFromCard = (card) => null;

            EffectEvents.OnAdded -= ExecuteOnAppliedEffect;
            EffectEvents.OnRemoved -= ExecuteOnAppliedEffect;
            EffectEvents.OnUpdated -= ExecuteOnAppliedEffect;

            CartEvents.OnNewCartPileUsed -= ExecuteNewCartPileEffects;
            CartEvents.OnStackHovered -= OnStackHovered;
        }

        void OnEnable()
        {
            EffectServices.AddEffect = Add;
            EffectServices.Cancel = Cancel;
            EffectServices.Execute = (trigger, card)
                        => StartCoroutine(Execute(trigger, card));
            EffectServices.ExecutePile = (trigger, cardPile)
                        => StartCoroutine(ExecutePile(trigger, cardPile));
            EffectServices.GetCurrent = () => new List<Effect>(_effects);

            EffectServices.ModifyValue = ModifyValue;
            EffectServices.SetValue = SetValue;
            EffectServices.GetValue = GetValue;

            EffectServices.IsCardPlayable = IsCardPlayable;


            EffectServices.RemoveEffectsLinkedToPiles = RemoveEffectsLinkedToPiles;
            EffectServices.CleanEffects = CleanEffect;

            EffectServices.GetCardPrice = GetCardPrice;
            EffectServices.GetStackPrice = GetStackPrice;


            EffectServices.CancelAllEffects = CancelAllEffects;
            EffectServices.ChangePreference = ChangePreference;
            EffectServices.CounterEffect = CounterEffect;

            EffectServices.GetCartSizeModifier = GetCartSizeModifier;

            EffectServices.RemoveArtifactEffects = RemoveArtifactEffects;
            EffectServices.GetSynergyBonuses = GetSynergyBonuses;

            EffectServices.UpdateCardEffects = UpdateCardEffects;
            EffectServices.AddEffectsFromCard = AddEffectsFromCard;

            EffectEvents.OnAdded += ExecuteOnAppliedEffect;
            EffectEvents.OnRemoved += ExecuteOnAppliedEffect;
            EffectEvents.OnUpdated += ExecuteOnAppliedEffect;

            CartEvents.OnNewCartPileUsed += ExecuteNewCartPileEffects;
            CartEvents.OnStackHovered += OnStackHovered;
        }

        private IEnumerator AddEffectsFromCard(Card card)
        {

            foreach (var effect in card.Effects)
            {
                if (effect.Trigger != EnumEffectTrigger.Passive) continue;

                Add(effect);
                yield return new WaitForSeconds(0.5f);

            }
        }

        private void OnStackHovered(Card card, CardPile pile)
        {
            //TODO: Show the effects of the hovered card
        }

        private void ExecuteNewCartPileEffects(Card card)
        {
            StartCoroutine(Execute(EnumEffectTrigger.OnNewCartPileUsed, card));
        }

        private void UpdateCardEffects(List<Card> topCardList)
        {
            var effectToRemove = _effects.Where(e => e.Tags.Contains(EnumEffectTag.ItemCard) &&
                        !topCardList.Contains(e.LinkedCard)).ToList();
            foreach (var effect in effectToRemove)
            {
                if (!effect.Tags.Contains(EnumEffectTag.ItemCard)) continue;
                if (!topCardList.Contains(effect.LinkedCard))
                    _effects.Remove(effect);
                EffectEvents.OnRemoved?.Invoke(effect);
            }
        }

        private (int multiplier, int bonus) GetSynergyBonuses(Card card, List<Item> list)
        {
            bool synergyPredicate(Effect effect) =>
                effect.Data is StackMultiplierEffect synergyEffect &&
                (synergyEffect.Category == card.Item.Category ||
                    synergyEffect.Category == EnumItemCategory.Any);

            //   var synergyEffects = card.Effects.FindAll(synergyPredicate);

            var synergyEffects = _effects.FindAll(synergyPredicate);

            if (synergyEffects.Count == 0) return (list.Count + 1, 0);

            int multiplier = synergyEffects
                .Where(effect => effect.Data is StackMultiplierEffect synergyEffect &&
                        synergyEffect.Operation == EnumOperation.Multiply)
                .Sum(effect => effect.Value);

            int bonus = synergyEffects
                .Where(effect => effect.Data is StackMultiplierEffect synergyEffect &&
                        synergyEffect.Operation == EnumOperation.Add)
                .Sum(effect => effect.Value);

            return (multiplier * list.Count + 1, bonus);

        }

        private void RemoveArtifactEffects(ArtifactData data)
        {
            if (data == null) return;

            var effectsToRemove = _effects.FindAll(effect =>
                effect.LinkedArtifact == data);

            foreach (var effect in effectsToRemove)
            {
                _effects.Remove(effect);
                EffectEvents.OnRemoved?.Invoke(effect);
            }
        }

        private int CounterEffect(EffectData data, int valueToCounter)
        {
            valueToCounter = Mathf.Abs(valueToCounter);
            var counterEffect = _effects.Find(effect => effect.Data == data);

            if (counterEffect != null)
            {
                int counteredValue = Math.Abs(counterEffect.Value);
                counteredValue = Math.Min(valueToCounter, counteredValue);
                ModifyValue(data, -counteredValue);
                return counteredValue;
            }
            return 0;

        }

        private bool IsCardPlayable(Card card)
        {
            if (card == null) return false;

            var requirements = card.Effects.FindAll(effect => effect.Data is IEffectRequirement);

            requirements.AddRange(_effects.FindAll(effect => effect.Data is IEffectRequirement));

            return requirements.TrueForAll(r => (r.Data as IEffectRequirement).IsFulfilled(r, card));
        }

        private int GetStackPrice(Card topCard, List<Item> stack)
        {
            if (topCard == null || stack == null || stack.Count == 0) return 0;



            List<Effect> stackEffects = _effects.FindAll(effect => effect.Data is StackMultiplierEffect stackEffect &&
                                               (effect.Trigger == EnumEffectTrigger.Passive) &&
                                               (stackEffect.Category == topCard.Item.Category || stackEffect.Category == EnumItemCategory.Any));


            stackEffects.AddRange(topCard.Effects.FindAll(effect => effect.Data is StackMultiplierEffect stackEffect &&
                                        (effect.Trigger == EnumEffectTrigger.Passive) &&
                                        (!effect.Tags.Contains(EnumEffectTag.Status)) &&
                                        (stackEffect.Category == topCard.Item.Category || stackEffect.Category == EnumItemCategory.Any)));
            if (stackEffects.Count == 0) return 0;

            int stackPrice = 0;
            foreach (var item in stack)
            {
                int stackBonus = stackEffects.Where(effect => effect.Data is StackMultiplierEffect stackEffect &&
                                (stackEffect.Category == item.Category || stackEffect.Category == EnumItemCategory.Any))
                                .Sum(effect => effect.Value);
                stackPrice += topCard.Price * stackBonus;
            }

            return stackPrice;
        }

        private int GetCartSizeModifier()
        {
            int cartSizeModifier = 0;

            foreach (var effect in _effects)
            {
                if (effect.Data is StressEffect)
                    cartSizeModifier -= effect.Value;

                if (effect.Data is CartSizeModifierEffect)
                    cartSizeModifier += effect.Value;
            }


            return cartSizeModifier;
        }

        private int GetCardPrice(Card card)
        {
            if (card == null) return 0;

            int priceModifier = GetPriceModifier(card);
            float ratioModifier = GetPriceRatioModifier(card);

            int basePrice = card.Item.BasePrice;

            if (card.Effects.Exists(e => e.Data is CategoryInCartPriceEffect))
            {
                var effect = card.Effects.Find(e => e.Data is CategoryInCartPriceEffect);

                int numCards = CartServices.GetNumCardInCart((effect.Data as ICategoryEffect).Category);
                basePrice = numCards * effect.Value;
            }

            int finalPrice = Mathf.RoundToInt((basePrice + priceModifier) * ratioModifier);

            return finalPrice;
        }

        private void ChangePreference(EnumItemCategory category)
        {
            var effect = _effects.Find(effect => effect.Data is ValuePriceEffect boostPrice &&
                    effect.ContainsTag(EnumEffectTag.Client));
            if (effect != null)
            {
                _effects.Remove(effect);
                EffectEvents.OnRemoved?.Invoke(effect);
            }

            var effectData = _effectCollection.Find(effectData =>
            effectData is ValuePriceEffect boostPriceEffect &&
            boostPriceEffect.Category == category
            );
            var newEffect = new Effect(effectData);
            newEffect.Tags.Add(EnumEffectTag.Client);
            Add(newEffect);
        }

        private int GetValue(EffectData data)
        {
            var existingEffect = _effects.Find(e
                => e.Data == data);
            if (existingEffect != null)
            {
                return existingEffect.Value;
            }
            return 0;
        }

        private void SetValue(EffectData data, int value)
        {
            var existingEffect = _effects.Find(e
                => e.Data == data);

            if (value == 0)
            {
                if (existingEffect != null)
                {
                    _effects.Remove(existingEffect);
                    EffectEvents.OnRemoved?.Invoke(existingEffect);
                }
                return;
            }
            if (existingEffect != null)
            {
                existingEffect.Value = value;
                EffectEvents.OnUpdated?.Invoke(existingEffect);
            }
            else
            {
                var newEffect = new Effect(data, value);
                _effects.Add(newEffect);
                EffectEvents.OnAdded?.Invoke(newEffect);
            }
        }

        private void ModifyValue(EffectData data, int value)
        {
            var existingEffect = _effects.Find(e
                => e.Data == data);
            if (existingEffect != null)
            {
                if (!data.CanBeNegative && existingEffect.Value + value <= 0)
                {
                    _effects.Remove(existingEffect);
                    EffectEvents.OnRemoved?.Invoke(existingEffect);
                    return;
                }
                existingEffect.Value += value;
                EffectEvents.OnUpdated?.Invoke(existingEffect);
            }
            else
            {
                if (!data.CanBeNegative && value <= 0)
                {
                    return;
                }
                var newEffect = new Effect(data, value);

                newEffect.Tags.Add(EnumEffectTag.Status);



                _effects.Add(newEffect);
                EffectEvents.OnAdded?.Invoke(newEffect);
            }
        }

        private void CancelAllEffects(List<EffectData> whiteList)
        {
            var effectToCancel = _effects.FindAll(effect => !whiteList.Contains(effect.Data));

            foreach (var effect in effectToCancel)
                Cancel(effect.Data);
        }

        private void ExecuteOnAppliedEffect(Effect _)
        {
            //TODO:: Check who is using this and if it is still needed
            // foreach (var effect in _effects)
            //     if (effect.Trigger == EnumEffectTrigger.OnEffectApplied)
            //         effect.Execute(null);
        }

        private void CleanEffect()
        {
            var effectToClean = new List<Effect>(_effects); // _effects.FindAll(effect =>
                                                            //effect.Tags.Contains(EnumEffectTag.Client) ||
                                                            // effect.Tags.Contains(EnumEffectTag.Card));

            foreach (var effect in effectToClean)
            {
                _effects.Remove(effect);
                EffectEvents.OnRemoved?.Invoke(effect);
            }
        }

        private int GetPriceModifier(Card card)
        {
            int priceModifier = 0;
            Assert.IsNotNull(card, "Card cannot be null when calculating price modifier.");

            foreach (var effect in _effects)
            {
                if (effect.Data is not IPriceModifierEffect modifierEffectData)
                    continue;


                //Look for all possible amplifiers for this effect
                int effectAmplifiers = _effects.FindAll(e => e.Data is EffectAmplifierEffect amplifierEffect &&
                                       amplifierEffect.EffectToAmplify == effect.Data).Sum<Effect>(e => e.Value);
                effectAmplifiers += card.Effects.FindAll(e => e.Data is EffectAmplifierEffect amplifierEffect &&
               amplifierEffect.EffectToAmplify == effect.Data).Sum<Effect>(e => e.Value);

                if (effectAmplifiers <= 0)
                    effectAmplifiers = 1; // Ensure at least one multiplier is applied

                priceModifier += modifierEffectData.PriceModifier(effect, card) * effectAmplifiers;
            }

            foreach (var effect in card.Effects)
            {
                if (effect.Data == null)
                {
                    Debug.LogWarning($"Effect is null when calculating price modifier: {card.Item.Data.name}");
                    continue;
                }
                if (effect.Data is not IPriceModifierEffect modifierEffectData)
                    continue;
                int effectAmplifiers = _effects.FindAll(e => e.Data is EffectAmplifierEffect amplifierEffect &&
                                                        amplifierEffect.EffectToAmplify == effect.Data).Sum<Effect>(e => e.Value);
                effectAmplifiers += card.Effects.FindAll(e => e.Data is EffectAmplifierEffect amplifierEffect &&
                        amplifierEffect.EffectToAmplify == effect.Data).Sum<Effect>(e => e.Value);

                if (effectAmplifiers <= 0)
                    effectAmplifiers = 1; // Ensure at least one multiplier is applied

                //                Debug.Log($"Effect: {effect.Data.name}, Amplifiers: {effectAmplifiers}, Price Modifier: {effect.PriceModifier(card)}");
                priceModifier += modifierEffectData.PriceModifier(effect, card) * effectAmplifiers;
            }

            return priceModifier;
        }

        private float GetPriceRatioModifier(Card card)
        {
            float priceMultiplier = 0f;
            foreach (var effect in _effects)
            {
                if (effect.Data is not IPriceModifierEffect modifierEffectData)
                    continue;
                priceMultiplier += modifierEffectData.PriceMultiplier(effect, card);
            }
            foreach (var effect in card.Effects)
            {
                if (effect.Data == null)
                {
                    Debug.LogWarning($"Effect is null when calculating price ratio modifier: {card.Item.Data.name}");
                    continue;
                }
                if (effect.Data is not IPriceModifierEffect modifierEffectData)
                    continue;
                priceMultiplier += modifierEffectData.PriceMultiplier(effect, card);
            }
            return (priceMultiplier == 0f) ? 1f : priceMultiplier;
        }

        private void RemoveEffectsLinkedToPiles(List<CardPile> list)
        {

            foreach (var pile in list)
            {
                if (pile == null) continue;
                List<int> idsToRemove = new();
                var effectToRemove = _effects.FindAll(effect =>
                    effect.Tags.Contains(EnumEffectTag.ItemCard));

                foreach (var effect in effectToRemove)
                {
                    _effects.Remove(effect);
                    EffectEvents.OnRemoved?.Invoke(effect);
                }
            }
        }

        public void Add(Effect effect)
        {

            var existingEffect = _effects.Find(e
                => e.Data == effect.Data &&
                     e.Trigger == effect.Trigger);

            if (existingEffect != null)
            {
                existingEffect.Value += effect.Value;
                EffectEvents.OnUpdated?.Invoke(existingEffect);
                return;
            }
            else
            {
                _effects.Add(new(effect));
            }

            EffectEvents.OnAdded?.Invoke(effect);

        }


        private void Cancel(EffectData effectData)
        {
            if (effectData == null) return;

            var effect = _effects.Find(e => e.Data == effectData);
            if (effect == null) return;
            effect.Data.Cancel(effect);
            _effects.Remove(effect);
            EffectEvents.OnRemoved?.Invoke(effect);
        }

        private IEnumerator ExecutePile(EnumEffectTrigger trigger, CardPile pile)
        {
            if (pile.IsEmpty) yield break;
            var card = pile.TopCard;

            var effectToExecute = card.Effects.FindAll(effect => effect.Trigger == trigger);

            foreach (var effect in card.Effects)
            {
                if (effect.Trigger != trigger) continue;
                // effect.LinkedCard = card;

                yield return StartCoroutine(effect.Data.ExecutePile(effect, pile));
            }
        }

        private IEnumerator Execute(EnumEffectTrigger trigger, Card card)
        {
            var effectToExecute = _effects.FindAll(effect => effect.Trigger == trigger);
            if (card != null)
                effectToExecute.AddRange(card.Effects.FindAll(effect => effect.Trigger == trigger));

            foreach (var effect in effectToExecute)
            {
                //effect.LinkedCard = card;
                if (effect.Trigger == EnumEffectTrigger.Passive)
                {
                    Add(effect);
                    yield return new WaitForSeconds(0.5f);
                }
                else

                    yield return StartCoroutine(effect.Data.Execute(effect));
            }

        }
    }
}
