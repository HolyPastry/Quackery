using System;
using System.Collections;
using System.Collections.Generic;
using Quackery.Artifacts;
using Quackery.Decks;
using Quackery.Effects;
using UnityEngine;

namespace Quackery
{

    [Serializable]
    public class Effect
    {
        public EffectData Data;
        public int Value;
        public EnumEffectTrigger Trigger => Data.Trigger;

        [HideInInspector]
        public List<EnumEffectTag> Tags = new();

        public Sprite Icon => Data.Icon;

        public Card LinkedCard { get; set; }
        public ArtifactData LinkedArtifact { get; internal set; }

        public string Description => Data == null ?
                         "No Effect Data Assigned" :
                 Data.Description.Replace("#Value", Value.ToString());



        public Effect()
        { }

        public Effect(EffectData data)
        {
            Data = data;
        }

        // public int PriceModifier(Card card) => Data.PriceModifier(this, card);
        // public float PriceRatioModifier(Card card) => Data.RatioPriceModifier(this, card);

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
