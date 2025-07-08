
using System;
using System.Collections.Generic;
using Holypastry.Bakery;

using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;


namespace Quackery.Effects
{
    public interface ICategoryEffect
    {
        public EnumItemCategory Category { get; }
    }

    public interface IPriceModifierEffect
    {
        public int PriceModifier(Effect effect, Card card);

        public float PriceMultiplier(Effect effect, Card card);

    }
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
