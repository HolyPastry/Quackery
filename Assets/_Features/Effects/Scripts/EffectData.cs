
using System;
using System.Collections.Generic;
using Holypastry.Bakery;

using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;


namespace Quackery.Effects
{
    public abstract class CategoryEffectData : EffectData
    {
        public EnumItemCategory Category = EnumItemCategory.Unset;
    }
    public abstract class EffectData : ContentTag
    {
        public Sprite Icon;

        public string Description;

        public bool CanBeNegative = false;

        public List<Explanation> Explanations;

        public virtual void Execute(Effect effect) { }
        public virtual void Cancel(Effect effect) { }

        public virtual int PriceModifier(Effect effect, Card card) => 0;

        public virtual float RatioPriceModifier(Effect effect, Card card) => 0f;

        public virtual void ExecutePile(Effect effect, CardPile pile) { }

    }
}
