using System;
using System.Collections.Generic;
using System.Linq;
using Holypastry.Bakery;
using Ink.Parsed;
using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;
using UnityEngine.Android;
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
            EffectServices.AddStatus = (effectData) => { };
            EffectServices.Cancel = delegate { };

            EffectServices.Execute = (trigger, card) => 0;
            EffectServices.ExecutePile = (trigger, cardPile) => { };

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

            EffectEvents.OnAdded -= ExecuteOnAppliedEffect;
            EffectEvents.OnRemoved -= ExecuteOnAppliedEffect;
            EffectEvents.OnUpdated -= ExecuteOnAppliedEffect;

        }

        void OnEnable()
        {
            EffectServices.AddStatus = Add;
            EffectServices.Cancel = Cancel;
            EffectServices.Execute = Execute;
            EffectServices.ExecutePile = ExecutePile;
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

            EffectEvents.OnAdded += ExecuteOnAppliedEffect;
            EffectEvents.OnRemoved += ExecuteOnAppliedEffect;
            EffectEvents.OnUpdated += ExecuteOnAppliedEffect;
        }



        private int CounterEffect(EffectData data, int valueToCounter)
        {
            var counterEffect = _effects.Find(effect => effect.Data == data);
            if (counterEffect != null)
            {
                var counteredValue = Math.Min(0, counterEffect.Value - valueToCounter);
                counterEffect.Value -= counteredValue;
                EffectEvents.OnUpdated?.Invoke(counterEffect);
                return counteredValue;
            }
            return 0;

        }

        private bool IsCardPlayable(Card card)
        {
            if (card == null) return false;

            var requirements = card.Effects.FindAll(effect => effect.Data is RequirementEffectData);

            requirements.AddRange(_effects.FindAll(effect => effect.Data is RequirementEffectData));

            return requirements.TrueForAll(r => (r.Data as RequirementEffectData).IsFulfilled(r, card));
        }

        private int GetStackPrice(Card topCard, List<Item> stack)
        {
            if (topCard == null || stack == null || stack.Count == 0) return 0;



            List<Effect> stackEffects = _effects.FindAll(effect => effect.Data is StackMultiplierEffect stackEffect &&
                                               (effect.Trigger == EnumEffectTrigger.Passive) &&
                                               (stackEffect.Category == topCard.Item.Category || stackEffect.Category == EnumItemCategory.Unset));


            stackEffects.AddRange(topCard.Effects.FindAll(effect => effect.Data is StackMultiplierEffect stackEffect &&
                                        (effect.Trigger == EnumEffectTrigger.Passive) &&
                                        (!effect.Tags.Contains(EnumEffectTag.Status)) &&
                                        (stackEffect.Category == topCard.Item.Category || stackEffect.Category == EnumItemCategory.Unset)));
            if (stackEffects.Count == 0) return 0;

            int stackPrice = 0;
            foreach (var item in stack)
            {
                int stackBonus = stackEffects.Where(effect => effect.Data is StackMultiplierEffect stackEffect &&
                                (stackEffect.Category == item.Category || stackEffect.Category == EnumItemCategory.Unset))
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
                if (effect.Data is CartSizeModifierEffect)
                {
                    cartSizeModifier += effect.Value;
                }
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

                int numCards = CartServices.GetNumCardInCart((effect.Data as CategoryInCartPriceEffect).Category);
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
                var newEffect = new Effect(data) { Value = value };
                _effects.Add(newEffect);
                EffectEvents.OnAdded?.Invoke(newEffect);
            }
        }

        private void ModifyValue(EffectData data, int arg2)
        {
            var existingEffect = _effects.Find(e
                => e.Data == data);
            if (existingEffect != null)
            {
                if (!data.CanBeNegative && existingEffect.Value + arg2 <= 0)
                {
                    _effects.Remove(existingEffect);
                    EffectEvents.OnRemoved?.Invoke(existingEffect);
                    return;
                }
                existingEffect.Value += arg2;
                EffectEvents.OnUpdated?.Invoke(existingEffect);
            }
            else
            {
                if (!data.CanBeNegative && arg2 <= 0)
                {
                    return;
                }
                var newEffect = new Effect(data)
                {
                    Value = arg2,
                    Trigger = EnumEffectTrigger.Continous

                };
                newEffect.Tags.Add(EnumEffectTag.Activated);

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
            foreach (var effect in _effects)
                if (effect.Trigger == EnumEffectTrigger.OnEffectApplied)
                    effect.Execute(null);

        }

        private void CleanEffect()
        {
            var effectToClean = _effects.FindAll(effect =>
                effect.Tags.Contains(EnumEffectTag.Activated) ||
                effect.Tags.Contains(EnumEffectTag.Client));

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
                //Look for all possible amplifiers for this effect
                int effectAmplifiers = _effects.FindAll(e => e.Data is EffectAmplifierEffect amplifierEffect &&
                                           amplifierEffect.EffectToAmplify == effect.Data).Sum<Effect>(e => e.Value);
                effectAmplifiers += card.Effects.FindAll(e => e.Data is EffectAmplifierEffect amplifierEffect &&
               amplifierEffect.EffectToAmplify == effect.Data).Sum<Effect>(e => e.Value);

                if (effectAmplifiers <= 0)
                    effectAmplifiers = 1; // Ensure at least one multiplier is applied

                priceModifier += effect.PriceModifier(card) * effectAmplifiers;
            }

            foreach (var effect in card.Effects)
            {
                if (effect.Data == null)
                {
                    Debug.LogWarning($"Effect is null when calculating price modifier: {card.Item.Data.name}");
                    continue;
                }
                int effectAmplifiers = _effects.FindAll(e => e.Data is EffectAmplifierEffect amplifierEffect &&
                                                        amplifierEffect.EffectToAmplify == effect.Data).Sum<Effect>(e => e.Value);
                effectAmplifiers += card.Effects.FindAll(e => e.Data is EffectAmplifierEffect amplifierEffect &&
                        amplifierEffect.EffectToAmplify == effect.Data).Sum<Effect>(e => e.Value);

                if (effectAmplifiers <= 0)
                    effectAmplifiers = 1; // Ensure at least one multiplier is applied

                //                Debug.Log($"Effect: {effect.Data.name}, Amplifiers: {effectAmplifiers}, Price Modifier: {effect.PriceModifier(card)}");
                priceModifier += effect.PriceModifier(card) * effectAmplifiers;
            }

            return priceModifier;
        }

        private float GetPriceRatioModifier(Card card)
        {
            float priceRatioModifier = 1.0f;
            foreach (var effect in _effects)
            {
                priceRatioModifier += effect.PriceRatioModifier(card);
            }
            foreach (var effect in card.Effects)
            {
                priceRatioModifier += effect.PriceRatioModifier(card);
            }
            return priceRatioModifier;
        }

        private void RemoveEffectsLinkedToPiles(List<CardPile> list)
        {

            foreach (var pile in list)
            {
                if (pile == null) continue;
                List<int> idsToRemove = new();
                var effectToRemove = _effects.FindAll(effect =>
                    effect.LinkedCard != null &&
                    effect.Tags.Contains(EnumEffectTag.Activated) &&
                    effect.LinkedCard == pile.TopCard);

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
                _effects.Add(effect);
                if (effect.Trigger == EnumEffectTrigger.Continous)
                    effect.Execute(null);
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

        private void ExecutePile(EnumEffectTrigger trigger, CardPile pile)
        {
            if (pile.IsEmpty) return;
            var card = pile.TopCard;

            var effectToExecute = card.Effects.FindAll(effect => effect.Trigger == trigger);

            foreach (var effect in card.Effects)
            {
                if (effect.Trigger != trigger) continue;
                effect.LinkedCard = card;
                effect.ExecutePile(pile);
            }
        }

        private int Execute(EnumEffectTrigger trigger, Card card)
        {
            // List<int> _effectToRemove = new();
            // var effectToExecute = _effects.FindAll(effect => effect.Trigger == trigger);

            // foreach (var effect in effectToExecute)
            // {
            //     if (effect.Trigger != trigger) continue;
            //     effect.Execute(pile);
            //     if (effect.Tags.Contains(EnumEffectTag.OneTime))
            //     {
            //         _effects.Remove(effect);
            //         EffectEvents.OnRemoved?.Invoke(effect);
            //     }
            // }

            if (card == null)
            {
                Debug.LogWarning("Card is null when executing effects.");
                return 0; ;
            }

            var effectToExecute = card.Effects.FindAll(effect => effect.Trigger == trigger);

            foreach (var effect in card.Effects)
            {
                if (effect.Trigger != trigger) continue;
                effect.Execute(card);
            }
            return effectToExecute.Count();

        }
    }
}
