using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Holypastry.Bakery;

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
            EffectServices.Add = (effectCarrier) => null;
            EffectServices.Remove = (effect) => null;
            EffectServices.RemoveLinkedToObject = (linkedObject) => null;

            EffectServices.Execute = (trigger, card) => null;
            EffectServices.ExecutePile = (trigger, cardPile) => null;

            EffectServices.GetCurrent = () => new();

            EffectServices.CleanEffects = delegate { _effects.Clear(); };
            EffectServices.CounterEffect = (itemData, numCard) => 0;


            EffectServices.GetModifier = (effectDataType) => 0;
            EffectServices.UpdateDurationEffects = () => null;
            EffectServices.GetActiveStatuses = () => new();

            EffectEvents.OnAdded -= ExecuteOnAppliedEffect;
            EffectEvents.OnRemoved -= ExecuteOnAppliedEffect;
            EffectEvents.OnUpdated -= ExecuteOnAppliedEffect;

            CartEvents.OnNewCartPileUsed -= ExecuteNewCartPileEffects;
            CartEvents.OnStackHovered -= OnStackHovered;
        }

        void OnEnable()
        {
            EffectServices.Add = (effectCarrier) => StartCoroutine(AddEffectRoutine(effectCarrier));
            EffectServices.Remove = (predicate) => StartCoroutine(RemoveRoutine(predicate));
            EffectServices.RemoveLinkedToObject = (linkedObject)
                    => StartCoroutine(RemoveRoutine(e => e.LinkedObject == linkedObject &&
                    !e.Tags.Contains(EnumEffectTag.Persistent)));


            EffectServices.Execute = (trigger, effectCarrier)
                        => StartCoroutine(Execute(trigger, effectCarrier));
            EffectServices.ExecutePile = (trigger, cardPile)
                        => StartCoroutine(ExecutePile(trigger, cardPile));

            EffectServices.GetCurrent = () => new List<Effect>(_effects);

            EffectServices.CleanEffects = CleanEffect;
            EffectServices.CounterEffect = CounterEffect;

            EffectServices.GetModifier = GetModifier;
            EffectServices.UpdateDurationEffects = () => StartCoroutine(UpdateDurationEffects());
            EffectServices.GetActiveStatuses = GetActiveStatuses;

            EffectEvents.OnAdded += ExecuteOnAppliedEffect;
            EffectEvents.OnRemoved += ExecuteOnAppliedEffect;
            EffectEvents.OnUpdated += ExecuteOnAppliedEffect;

            CartEvents.OnNewCartPileUsed += ExecuteNewCartPileEffects;
            CartEvents.OnStackHovered += OnStackHovered;
        }

        private IEnumerator RemoveRoutine(Predicate<Effect> predicate)
        {
            var effects = _effects.FindAll(predicate);
            foreach (var effect in effects)
            {
                _effects.Remove(effect);
                EffectEvents.OnRemoved?.Invoke(effect);
                yield return Tempo.WaitForABeat;
            }
            ;
        }

        private Dictionary<Status, int> GetActiveStatuses()
        {
            var statuses = new Dictionary<Status, int>();

            foreach (var effect in _effects)
            {
                if (effect.Data is not IStatusEffect statusEffect)
                    continue;

                if (statuses.TryGetValue(statusEffect.Status, out int currentValue))
                    statuses[statusEffect.Status] = currentValue + (int)effect.Value;
                else
                    statuses.Add(statusEffect.Status, (int)effect.Value);
            }
            return statuses;
        }

        private float GetModifier(Type type)
        {
            var effects = _effects.Where(e => e.Data.GetType().Equals(type)).ToList();
            float modifier = 0;
            foreach (var e in effects)
            {
                modifier += e.Value;
                if (e.ContainsTag(EnumEffectTag.OneTime))
                    StartCoroutine(RemoveRoutine(e2 => e2 == e));
            }
            return modifier;
        }


        private void OnStackHovered(Card card, CardPile pile)
        {
            //TODO: Show the effects of the hovered card
        }

        private void ExecuteNewCartPileEffects(Card card)
        {
            // StartCoroutine(Execute(EnumEffectTrigger.OnNewCartPileUsed, card));
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

        private IEnumerator AddEffectRoutine(IEffectCarrier effectCarrier)
        {
            foreach (var effectData in effectCarrier.EffectDataList)
            {
                var effect = new Effect(effectData);
                _effects.Add(effect);
                effect.LinkedObject = effectCarrier;
                EffectEvents.OnAdded?.Invoke(effect);
                if (effect.Data is IStatusEffect)
                    yield return new WaitForSeconds(Tempo.Beat);
            }
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

        private IEnumerator Execute(EnumEffectTrigger trigger, IEffectCarrier carrier)
        {

            var effectToExecute = _effects
                .FindAll(effect =>

                            (effect.Data is IStatusEffect statusEffect &&
                            statusEffect.Trigger == trigger)

                            ||

                            (effect.Data is not IStatusEffect &&
                            trigger == EnumEffectTrigger.OnCardPlayed &&
                            effect.LinkedObject == carrier)
                        );



            foreach (var effect in effectToExecute)
            {
                yield return StartCoroutine(effect.Data.Execute(effect));
            }
        }

        private IEnumerator UpdateDurationEffects()
        {
            var durationEffects = _effects.FindAll(effect => effect.ContainsTag(EnumEffectTag.DecreaseOverTime));

            foreach (var effect in durationEffects)
            {
                effect.Value--;

                if (effect.Value <= 0)
                    yield return StartCoroutine(RemoveRoutine(e => e.Data == effect.Data));
                else
                {
                    EffectEvents.OnUpdated?.Invoke(effect);
                    yield return new WaitForSeconds(Tempo.Beat);
                }
            }
        }
    }
}
