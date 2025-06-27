
using System.Collections.Generic;
using Holypastry.Bakery;

using Quackery.Decks;
using UnityEngine;


namespace Quackery.Effects
{
    public abstract class EffectData : ContentTag
    {
        public Sprite Icon;

        public virtual void Execute(Effect effect, CardPile pile) { }
        public virtual void Cancel(Effect effect) { }

        public virtual int PriceModifier(Effect effect, Card card) => 0;

        public virtual float RatioPriceModifier(Effect effect, Card card) => 0f;

        public virtual string GetDescription() => Sprites.ReplaceCategories(Description);

        public string Description;


    }
}
