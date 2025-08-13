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
            EffectServices.Add = (effectData, linkedObject) => { };
            EffectServices.Remove = (effect) => { };
            EffectServices.RemoveLinkedToObject = (linkedObject) => { };

            EffectServices.Execute = (trigger, card) => null;
            EffectServices.ExecutePile = (trigger, cardPile) => null;

            EffectServices.GetCurrent = () => new();


            EffectServices.IsCardPlayable = (card) => true;

            EffectServices.GetCardPrice = (card) => 0;


            EffectServices.CleanEffects = delegate { _effects.Clear(); };


            EffectServices.CounterEffect = (itemData, numCard) => 0;


            EffectServices.GetSynergyBonuses = (card, subItems) => (0, 0);

            EffectServices.GetModifier = (effectDataType) => 0;
            EffectServices.UpdateDurationEffects = () => null;


            EffectEvents.OnAdded -= ExecuteOnAppliedEffect;
            EffectEvents.OnRemoved -= ExecuteOnAppliedEffect;
            EffectEvents.OnUpdated -= ExecuteOnAppliedEffect;

            CartEvents.OnNewCartPileUsed -= ExecuteNewCartPileEffects;
            CartEvents.OnStackHovered -= OnStackHovered;
        }

        void OnEnable()
        {
            EffectServices.Add = Add;
            EffectServices.Remove = Remove;
            EffectServices.RemoveLinkedToObject = RemoveLinkedToObject;
            EffectServices.Execute = (trigger, card)
                        => StartCoroutine(Execute(trigger, card));
            EffectServices.ExecutePile = (trigger, cardPile)
                        => StartCoroutine(ExecutePile(trigger, cardPile));

            EffectServices.GetCurrent = () => new List<Effect>(_effects);

            EffectServices.IsCardPlayable = IsCardPlayable;
            EffectServices.CleanEffects = CleanEffect;
            EffectServices.CounterEffect = CounterEffect;


            EffectServices.GetCardPrice = GetCardPrice;
            EffectServices.GetSynergyBonuses = GetSynergyBonuses;

            EffectServices.GetModifier = GetModifier;
            EffectServices.UpdateDurationEffects = () => StartCoroutine(UpdateDurationEffects());

            EffectEvents.OnAdded += ExecuteOnAppliedEffect;
            EffectEvents.OnRemoved += ExecuteOnAppliedEffect;
            EffectEvents.OnUpdated += ExecuteOnAppliedEffect;

            CartEvents.OnNewCartPileUsed += ExecuteNewCartPileEffects;
            CartEvents.OnStackHovered += OnStackHovered;
        }

        private void RemoveLinkedToObject(object obj)
        {
            Remove(e => e.LinkedObject == obj);
        }

        private void Remove(Predicate<Effect> predicate)
        {
            _effects.FindAll(predicate).ForEach(effect =>
            {
                _effects.Remove(effect);
                EffectEvents.OnRemoved?.Invoke(effect);
            });

        }

        private float GetModifier(Type type)
        {
            var effects = _effects.Where(e => e.Data.GetType().Equals(type)).ToList();
            float modifier = 0;
            foreach (var e in effects)
            {
                modifier += e.Value;
                if (e.ContainsTag(EnumEffectTag.OneTime))
                    Remove(e2 => e2 == e);
            }
            return modifier;
        }


        private void OnStackHovered(Card card, CardPile pile)
        {
            //TODO: Show the effects of the hovered card
        }

        private void ExecuteNewCartPileEffects(Card card)
        {
            StartCoroutine(Execute(EnumEffectTrigger.OnNewCartPileUsed, card));
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

            int multiplier = (int)synergyEffects
                .Where(effect => effect.Data is StackMultiplierEffect synergyEffect &&
                        synergyEffect.Operation == EnumOperation.Multiply)
                .Sum(effect => effect.Value);

            int bonus = (int)synergyEffects
                .Where(effect => effect.Data is StackMultiplierEffect synergyEffect &&
                        synergyEffect.Operation == EnumOperation.Add)
                .Sum(effect => effect.Value);

            return (multiplier * (list.Count + 1), bonus);

        }


        private int CounterEffect(EffectData data, int valueToCounter)
        {
            valueToCounter = Mathf.Abs(valueToCounter);
            var counterEffect = _effects.Find(effect => effect.Data == data);

            if (counterEffect != null)
            {
                float counteredValue = Math.Abs(counterEffect.Value);
                counteredValue = Math.Min(valueToCounter, counteredValue);
                // ModifyValue(data, -counteredValue);
                return (int)counteredValue;
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

                                               (stackEffect.Category == topCard.Item.Category || stackEffect.Category == EnumItemCategory.Any));


            stackEffects.AddRange(topCard.Effects.FindAll(effect => effect.Data is StackMultiplierEffect stackEffect &&
                                        (stackEffect.Category == topCard.Item.Category || stackEffect.Category == EnumItemCategory.Any)));
            if (stackEffects.Count == 0) return 0;

            float stackPrice = 0;
            foreach (var item in stack)
            {
                float stackBonus = stackEffects.Where(effect => effect.Data is StackMultiplierEffect stackEffect &&
                                (stackEffect.Category == item.Category || stackEffect.Category == EnumItemCategory.Any))
                                .Sum(effect => effect.Value);
                stackPrice += topCard.Price * stackBonus;
            }

            return (int)stackPrice;
        }

        private int GetCardPrice(Card card)
        {
            if (card == null) return 0;

            int priceModifier = GetPriceModifier(card);
            float ratioModifier = GetPriceRatioModifier(card);

            float basePrice = card.Item.BasePrice;

            if (card.Effects.Exists(e => e.Data is CategoryInCartPriceEffect))
            {
                var effect = card.Effects.Find(e => e.Data is CategoryInCartPriceEffect);

                int numCards = CartServices.GetNumCardInCart((effect.Data as ICategoryEffect).Category);
                basePrice = numCards * effect.Value;
            }

            int finalPrice = Mathf.RoundToInt((basePrice + priceModifier) * ratioModifier);

            return finalPrice;
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
            float priceModifier = 0;
            Assert.IsNotNull(card, "Card cannot be null when calculating price modifier.");

            foreach (var effect in _effects)
            {
                if (effect.Data is not IPriceModifierEffect modifierEffectData)
                    continue;


                //Look for all possible amplifiers for this effect
                float effectAmplifiers = _effects.FindAll(e => e.Data is EffectAmplifierEffect amplifierEffect &&
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
                float effectAmplifiers = _effects.FindAll(e => e.Data is EffectAmplifierEffect amplifierEffect &&
                                                        amplifierEffect.EffectToAmplify == effect.Data).Sum<Effect>(e => e.Value);
                effectAmplifiers += card.Effects.FindAll(e => e.Data is EffectAmplifierEffect amplifierEffect &&
                        amplifierEffect.EffectToAmplify == effect.Data).Sum<Effect>(e => e.Value);

                if (effectAmplifiers <= 0)
                    effectAmplifiers = 1; // Ensure at least one multiplier is applied

                //                Debug.Log($"Effect: {effect.Data.name}, Amplifiers: {effectAmplifiers}, Price Modifier: {effect.PriceModifier(card)}");
                priceModifier += modifierEffectData.PriceModifier(effect, card) * effectAmplifiers;
            }

            return Mathf.RoundToInt(priceModifier);
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

        public void Add(EffectData effectData, object linkedObject)
        {
            var effect = new Effect(effectData);
            _effects.Add(effect);
            effect.LinkedObject = linkedObject;

            EffectEvents.OnAdded?.Invoke(effect);

        }



        private void Cancel(Predicate<Effect> predicate)
        {
            _effects.FindAll(predicate).ForEach(e =>
            {
                e.Data.OnRemove(e);
                Remove(e2 => e2 == e);
            });
        }

        private IEnumerator ExecutePile(EnumEffectTrigger trigger, CardPile pile)
        {
            if (pile.IsEmpty) yield break;
            var card = pile.TopCard;

            var effectToExecute = card.Effects
                .FindAll(effect => effect.Data is IStatusEffect statusEffect &&
                         statusEffect.Trigger == trigger);

            foreach (var effect in effectToExecute)
                yield return StartCoroutine(effect.Data.ExecutePile(effect, pile));

        }

        private IEnumerator Execute(EnumEffectTrigger trigger, Card card)
        {

            var effectToExecute = _effects
                .FindAll(effect => effect.Data is IStatusEffect statusEffect &&
                                    statusEffect.Trigger == trigger);

            if (card != null)
                effectToExecute
                    .AddRange(card.Effects
                        .FindAll(effect => effect.Data is not IStatusEffect statusEffect));

            foreach (var effect in effectToExecute)
            {
                yield return StartCoroutine(effect.Data.Execute(effect));
            }
        }

        private IEnumerator UpdateDurationEffects()
        {
            var durationEffects = _effects.FindAll(effect => effect.ContainsTag(EnumEffectTag.Duration));

            foreach (var effect in durationEffects)
            {
                effect.Value--;

                if (effect.Value <= 0)
                    Cancel(e => e.Data == effect.Data);
                else
                    EffectEvents.OnUpdated?.Invoke(effect);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
}
