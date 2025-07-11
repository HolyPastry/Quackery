
using System;
using System.Collections.Generic;
using Holypastry.Bakery;

using Quackery.Decks;
using UnityEngine;


namespace Quackery.Effects
{
    public abstract class EffectData : ContentTag
    {
        public Sprite Icon;

        public string Description;

        public bool CanBeNegative = false;

        public EnumEffectTrigger Trigger = EnumEffectTrigger.OnCardPlayed;

        public List<Explanation> Explanations;

        public virtual void Execute(Effect effect) { }
        public virtual void Cancel(Effect effect) { }
        public virtual void ExecutePile(Effect effect, CardPile pile) { }

    }
}
