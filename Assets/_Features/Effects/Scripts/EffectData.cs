using System;
using System.Collections.Generic;
using Holypastry.Bakery;
using Ink.Parsed;
using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{
    public abstract class EffectData : ContentTag
    {
        public Sprite Icon;
        public EnumEffectTrigger Trigger;
        public int StartValue;
        public bool UseValue;
        public abstract void Execute(Effect effect, CardPile pile);

        public virtual int RatingModifier(Effect effect, Card card) => 0;

        public virtual int PriceModifier(Effect effect, Card card) => 0;
        public string Description;

        public List<EnumEffectTag> Tags = new();
    }
}
