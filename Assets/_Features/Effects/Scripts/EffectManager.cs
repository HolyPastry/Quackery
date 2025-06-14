using System;
using System.Collections.Generic;
using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{
    public class EffectManager : MonoBehaviour
    {
        private readonly Dictionary<int, Effect> _effects = new();

        private int _nextId = 0;

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
        }

        private int GetRatingModifier(Card card)
        {

            int ratingModifier = 0;
            foreach (var effect in _effects.Values)
            {
                if (effect.Type == EnumEffectType.Continuous ||
                     effect.Trigger == EnumEffectTrigger.Activated)
                {
                    ratingModifier += effect.RatingModifier(card);
                }
            }
            return ratingModifier;
        }

        private int GetPriceModifier(Card card)
        {
            int priceModifier = 0;
            foreach (var effect in _effects.Values)
            {
                if (effect.Type == EnumEffectType.Continuous ||
                     effect.Trigger == EnumEffectTrigger.Activated)
                {
                    priceModifier += effect.PriceModifier(card);
                }
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
                        effect.Data.Trigger == EnumEffectTrigger.Activated &&
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
                effect.Execute(trigger, pile);
                if (effect.Data.Type == EnumEffectType.OneTime)
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
