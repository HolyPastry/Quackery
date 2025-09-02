
using System;
using System.Collections;
using System.Collections.Generic;
using Holypastry.Bakery;

using Quackery.Decks;
using UnityEngine;


namespace Quackery.Effects
{

    public abstract class EffectData : ScriptableObject
    {
        public EnumEffectTrigger Trigger;

        public List<EnumEffectTag> Tags;

        public virtual IEnumerator Execute(Effect effect) { yield break; }
        public virtual IEnumerator ExecutePile(Effect effect, CardPile pile) => null;

        public virtual void OnRemove(Effect effect) { }
        internal virtual void CheckValidity() { }

        public virtual void Setup(Effect effect)
        {
            effect.Tags.AddUniqueRange(Tags);
        }
    }
}
