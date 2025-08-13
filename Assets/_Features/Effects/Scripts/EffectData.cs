
using System;
using System.Collections;
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



        public List<EnumEffectTag> _tags;
        public List<Explanation> Explanations;
        public WaitForSeconds DefaultWaitTime = new(0.5f);

        public float Value;

        public virtual IEnumerator Execute(Effect effect) { yield break; }
        public virtual IEnumerator ExecutePile(Effect effect, CardPile pile) => null;

        public virtual void OnRemove(Effect effect) { }
        internal virtual void CheckValidity() { }

        public virtual void Setup(Effect effect)
        {
            effect.Tags.AddUniqueRange(_tags);
        }
    }
}
