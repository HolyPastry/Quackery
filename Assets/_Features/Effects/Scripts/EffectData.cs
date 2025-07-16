
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

        public EnumEffectTrigger Trigger = EnumEffectTrigger.OnCardPlayed;
        public List<Explanation> Explanations;
        public WaitForSeconds DefaultWaitTime = new(0.5f);

        public virtual IEnumerator Execute(Effect effect) { yield break; }
        public virtual IEnumerator ExecutePile(Effect effect, CardPile pile) => null;

        public virtual void Cancel(Effect effect) { }
        internal virtual void CheckValidity() { }
    }
}
