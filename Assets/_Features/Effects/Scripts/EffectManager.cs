using System;
using System.Collections.Generic;
using Holypastry.Bakery;
using Quackery.Decks;
using Quackery.Inventories;
using Quackery.Ratings;
using UnityEngine;

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
            EffectServices.Add = (effectData) => { };
            EffectServices.Cancel = delegate { };

            EffectServices.Execute = delegate { };
            EffectServices.GetCurrent = () => new();

            EffectServices.ModifyValue = (effectData, value) => { };
            EffectServices.SetValue = (effectData, value) => { };
            EffectServices.GetValue = (effectData) => 1;


            EffectServices.RemoveEffectsLinkedToPiles = delegate { };
            EffectServices.GetCardPrice = (card) => 0;

            EffectServices.CleanEffects = delegate { _effects.Clear(); };

            EffectServices.CancelAllEffects = delegate { };
            EffectServices.ChangePreference = (category) => { };

            EffectEvents.OnAdded -= ExecuteOnAppliedEffect;
            EffectEvents.OnRemoved -= ExecuteOnAppliedEffect;
            EffectEvents.OnUpdated -= ExecuteOnAppliedEffect;


        }

        void OnEnable()
        {
            EffectServices.Add = Add;
            EffectServices.Cancel = Cancel;
            EffectServices.Execute = Execute;
            EffectServices.GetCurrent = () => new List<Effect>(_effects);


            EffectServices.ModifyValue = ModifyValue;
            EffectServices.SetValue = SetValue;
            EffectServices.GetValue = GetValue;



            EffectServices.RemoveEffectsLinkedToPiles = RemoveEffectsLinkedToPiles;
            EffectServices.CleanEffects = CleanEffect;

            EffectServices.GetCardPrice = GetCardPrice;


            EffectServices.CancelAllEffects = CancelAllEffects;
            EffectServices.ChangePreference = ChangePreference;

            EffectEvents.OnAdded += ExecuteOnAppliedEffect;
            EffectEvents.OnRemoved += ExecuteOnAppliedEffect;
            EffectEvents.OnUpdated += ExecuteOnAppliedEffect;
        }



        private int GetCardPrice(Card card)
        {
            if (card == null) return 0;

            int priceModifier = GetPriceModifier(card);
            float ratioModifier = GetPriceRatioModifier(card);

            int basePrice = card.Item.Price;

            if (_effects.Exists(effect => effect.Data is PriceIsRatingEffect))
                basePrice = RatingServices.GetRating();

            int finalPrice = Mathf.RoundToInt((basePrice + priceModifier) * ratioModifier);

            return Mathf.Max(finalPrice, 0); // Ensure price is not negative
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
                existingEffect.Value += arg2;
                EffectEvents.OnUpdated?.Invoke(existingEffect);
            }
            else
            {
                var newEffect = new Effect(data) { Value = arg2 };
                _effects.Add(newEffect);
                EffectEvents.OnAdded?.Invoke(newEffect);
            }
        }

        private int GetValue()
        {
            throw new NotImplementedException();
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

        private void ModifyConfidence(int value)
        {
            var confidenceEffect = _effects.Find(effect => effect.Data is ConfidenceEffect);
            if (confidenceEffect == null) return;

            confidenceEffect.Value += value;
            EffectEvents.OnUpdated?.Invoke(confidenceEffect);

        }
        private void SetConfidence(int value)
        {
            var confidenceEffect = _effects.Find(effect => effect.Data is ConfidenceEffect);
            if (confidenceEffect == null) return;

            confidenceEffect.Value = value;
            EffectEvents.OnUpdated?.Invoke(confidenceEffect);
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
            foreach (var effect in _effects)
            {
                priceModifier += effect.PriceModifier(card);
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
            _effects.Add(effect);
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

        private void Execute(EnumEffectTrigger trigger, CardPile pile)
        {
            List<int> _effectToRemove = new();
            var effectToExecute = _effects.FindAll(effect => effect.Trigger == trigger);


            foreach (var effect in effectToExecute)
            {
                if (effect.Trigger != trigger) continue;
                effect.Execute(pile);
                if (effect.Tags.Contains(EnumEffectTag.OneTime))
                {
                    _effects.Remove(effect);
                    EffectEvents.OnRemoved?.Invoke(effect);
                }
            }
        }
    }
}
