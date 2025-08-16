using System;
using System.Collections;
using System.Collections.Generic;
using Quackery.Artifacts;
using Quackery.Decks;
using Quackery.Effects;
using UnityEngine;

namespace Quackery.Effects
{

    [Serializable]
    public class Effect
    {
        private bool _isInitialized;
        [SerializeField] private EffectData _data;
        public EffectData Data
        {
            get
            {
                if (_data != null && !_isInitialized)
                    Initialize();

                return _data;
            }
            set
            {
                _data = value;
                if (_data != null && !_isInitialized)
                    Initialize();
            }
        }

        public void Initialize()
        {
            if (_data != null)
            {
                _data.Setup(this);
                _isInitialized = true;
            }
        }

        public float Value { get; internal set; }

        //public EnumEffectTrigger Trigger => Data.Trigger;

        [HideInInspector]
        public List<EnumEffectTag> Tags = new();

        //public Sprite Icon => Data.Icon;

        public object LinkedObject;

        public string Description => Data == null ?
                         "No Effect Data Assigned" :
                 Data.Description.Replace("#Value", Value.ToString());

        public Effect()
        {
        }

        public Effect(EffectData data)
        {
            Data = data;
            Value = data.Value;
        }

        public Effect(Effect effect)
        {
            Data = effect.Data;
            Value = effect.Value;
            Tags = new List<EnumEffectTag>(effect.Tags);
            LinkedObject = effect.LinkedObject;

        }

        public Effect(EffectData data, int value)
        {
            Data = data;
            Value = value;
        }

        internal bool ContainsTag(EnumEffectTag effectTag)
        {
            return Tags.Contains(effectTag);
        }

        internal void CheckValidity()
        {
            if (Data == null)
            {
                Debug.LogWarning($"Effect {this} has no Data assigned.");
                return;
            }
            Data.CheckValidity();
        }
    }
}
