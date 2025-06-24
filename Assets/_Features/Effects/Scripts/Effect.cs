using System;
using System.Collections.Generic;
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
        public EnumEffectTrigger Trigger;
        public List<EnumEffectTag> Tags = new();

        public Sprite Icon => Data.Icon;

        public Card LinkedCard { get; set; }

        public string Description => Data == null ?
                         "No Effect Data Assigned" :
                 Data.GetDescription().Replace("#Value", Value.ToString());

        public Effect()
        { }

        public Effect(EffectData data)
        {
            Data = data;
        }

        internal void Execute(CardPile pile)
        {
            if (pile != null && !pile.IsEmpty) LinkedCard = pile.TopCard;
            Data.Execute(this, pile);
        }

        public int PriceModifier(Card card) => Data.PriceModifier(this, card);
        public float PriceRatioModifier(Card card) => Data.RatioPriceModifier(this, card);

        internal bool ContainsTag(EnumEffectTag effectTag)
        {
            return Tags.Contains(effectTag);
        }
    }
}
