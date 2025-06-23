using System;
using System.Collections.Generic;
using Holypastry.Bakery;
using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.Effects
{
    public class EffectManager : MonoBehaviour
    {
        private readonly Dictionary<int, Effect> _effects = new();

        private DataCollection<EffectData> _effectCollection;

        private int _nextId = 0;

        private int _stackRewardMultiplier = 1;

        void Awake()
        {
            _effectCollection = new("Effects");
        }

        void OnDisable()
        {
            EffectServices.Add = (effectData) => -1;
            EffectServices.Remove = delegate { };
            EffectServices.RemoveById = delegate { };

            EffectServices.UpdateValue = delegate { };
            EffectServices.Execute = delegate { };
            EffectServices.GetCurrent = () => new();

            EffectServices.RemoveEffectsLinkedToPiles = delegate { };
            EffectServices.GetPriceModifier = (card) => 0;
            EffectServices.GetRatingModifier = (card) => 0;

            EffectServices.CleanEffects = delegate { _effects.Clear(); _nextId = 0; };

            EffectServices.IncreaseStackReward = delegate { };
            EffectServices.GetStackMultiplier = () => 1;

            EffectServices.ModifyConfidence = (value) => { };
            EffectServices.RemoveAllEffects = delegate { };
            EffectServices.ChangePreference = (category) => { };

            EffectEvents.OnAdded -= ExecuteOnAppliedEffect;
            EffectEvents.OnRemoved -= ExecuteOnAppliedEffect;
            EffectEvents.OnUpdated -= ExecuteOnAppliedEffect;


        }

        void OnEnable()
        {
            EffectServices.Add = Add;
            EffectServices.Remove = Remove;
            EffectServices.RemoveById = RemoveById;

            EffectServices.UpdateValue = UpdateValue;
            EffectServices.Execute = Execute;
            EffectServices.GetCurrent = () => new List<Effect>(_effects.Values);

            EffectServices.RemoveEffectsLinkedToPiles = RemoveEffectsLinkedToPiles;
            EffectServices.GetPriceModifier = GetPriceModifier;
            EffectServices.GetRatingModifier = GetRatingModifier;

            EffectServices.CleanEffects = CleanEffect;
            EffectServices.IncreaseStackReward = IncreaseStackReward;
            EffectServices.GetStackMultiplier = () => _stackRewardMultiplier;

            EffectServices.ModifyConfidence = (value) => ModifyConfidence(value);
            EffectServices.RemoveAllEffects = RemoveAllEffects;
            EffectServices.ChangePreference = ChangePreference;

            EffectEvents.OnAdded += ExecuteOnAppliedEffect;
            EffectEvents.OnRemoved += ExecuteOnAppliedEffect;
            EffectEvents.OnUpdated += ExecuteOnAppliedEffect;
        }

        private void ChangePreference(EnumItemCategory category)
        {
            if (_effects.RemoveValue(effect => effect.Data is BoostPriceEffect boostPrice &&
                    effect.ContainsTag(EnumEffectTag.Client), out var removed))
                EffectEvents.OnRemoved?.Invoke(removed);


            var effectData = _effectCollection.Find(effectData =>
            effectData is BoostPriceEffect boostPriceEffect &&
            boostPriceEffect.Category == category &&
            effectData.Tags.Contains(EnumEffectTag.Client));

            Add(new Effect(effectData, true));
        }

        private void RemoveAllEffects()
        {

            _stackRewardMultiplier = 1;
            EffectEvents.OnStackMultiplerUpdate?.Invoke(_stackRewardMultiplier);

            foreach (var key in _effects.Keys)
            {
                RemoveById(key);
            }
        }

        private void ExecuteOnAppliedEffect(Effect _)
        {
            foreach (var effect in _effects.Values)
            {
                if (effect.Data.Trigger == EnumEffectTrigger.OnEffectApplied)
                {
                    effect.Execute(null);
                }
            }
        }

        private void ModifyConfidence(int value)
        {
            foreach (var effect in _effects.Values)
            {
                if (effect.Data is not ConfidenceEffect) return;
                {
                    effect.Value += value;
                    EffectEvents.OnUpdated?.Invoke(effect);
                }
            }
        }

        private void IncreaseStackReward(int increase)
        {
            _stackRewardMultiplier += increase;
            EffectEvents.OnStackMultiplerUpdate?.Invoke(_stackRewardMultiplier);
        }

        private void CleanEffect()
        {
            var keysToRemove = new List<int>();

            foreach (var (key, effect) in _effects)
            {
                if (effect.Data.Tags.Contains(EnumEffectTag.Activated))
                    keysToRemove.Add(key);
                if (effect.Data.Tags.Contains(EnumEffectTag.Client))
                    keysToRemove.Add(key);
            }

            _stackRewardMultiplier = 1;
            EffectEvents.OnStackMultiplerUpdate?.Invoke(_stackRewardMultiplier);
            foreach (var key in keysToRemove)
            {
                RemoveById(key);
            }
        }

        private int GetRatingModifier(Card card)
        {

            int ratingModifier = 0;
            foreach (var effect in _effects.Values)
            {
                ratingModifier += effect.RatingModifier(card);
            }
            return ratingModifier;
        }

        private int GetPriceModifier(Card card)
        {
            int priceModifier = 0;
            foreach (var effect in _effects.Values)
            {
                priceModifier += effect.PriceModifier(card);
            }
            return priceModifier;
        }

        private void RemoveEffectsLinkedToPiles(List<CardPile> list)
        {

            foreach (var pile in list)
            {
                if (pile == null) continue;
                List<int> idsToRemove = new();
                foreach (var (id, effect) in _effects)
                {
                    if (effect.LinkedCard != null &&
                        !pile.IsEmpty &&
                        effect.Data.Tags.Contains(EnumEffectTag.Activated) &&
                        effect.LinkedCard == pile.TopCard)
                    {
                        idsToRemove.Add(id);
                    }
                }
                foreach (var id in idsToRemove)
                {
                    RemoveById(id);
                }
            }
        }

        private void RemoveById(int id)
        {
            if (!_effects.ContainsKey(id))
            {
                Debug.LogWarning($"Status with ID {id} not found.");
                return;
            }

            var effect = _effects[id];
            _effects.Remove(id);
            EffectEvents.OnRemoved?.Invoke(effect);
        }

        public int Add(Effect effect)
        {

            int effectId = _nextId++;
            _effects.Add(effectId, effect);

            EffectEvents.OnAdded?.Invoke(effect);
            return effectId;
        }

        public void Remove(Effect effectToRemove)
        {
            if (!_effects.ContainsValue(effectToRemove)) return;
            foreach (var (key, effect) in _effects)
            {
                if (effect == effectToRemove)
                {
                    _effects.Remove(key);
                    EffectEvents.OnRemoved?.Invoke(effect);
                    return;
                }
            }
        }

        private void Execute(EnumEffectTrigger trigger, CardPile pile)
        {
            List<int> _effectToRemove = new();
            foreach (var (key, effect) in _effects)
            {
                if (effect.Trigger != trigger) continue;
                effect.Execute(pile);
                if (effect.Data.Tags.Contains(EnumEffectTag.OneTime))
                    _effectToRemove.Add(key);
            }
            foreach (var id in _effectToRemove)
            {
                RemoveById(id);
            }
        }

        public void UpdateValue(int effectId, int value)
        {
            if (!_effects.ContainsKey(effectId))
            {
                Debug.LogWarning($"Effect with ID {effectId} not found.");
                return;
            }
            _effects[effectId].Value = value;

            EffectEvents.OnUpdated?.Invoke(_effects[effectId]);

        }
    }
}
