using System;
using System.Collections;
using System.Collections.Generic;

using Quackery.Decks;
using Quackery.Effects;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery
{
    public static class EffectServices
    {
        public static Action<EffectData, object> Add = (effect, linkedObject) => { };
        public static Action<Predicate<Effect>> Remove = (predicate) => { };
        public static Action<object> RemoveLinkedToObject = (linkedObject) => { };

        public static Func<Type, float> GetModifier = (effectData) => 0;

        public static Func<List<Effect>> GetCurrent = () => new();

        internal static Func<EnumEffectTrigger, Card, Coroutine> Execute = (trigger, card) => null;
        public static Func<EnumEffectTrigger, CardPile, Coroutine> ExecutePile = (trigger, cardPile) => null;

        internal static Action CleanEffects = delegate { };

        internal static Func<Card, int> GetCardPrice = (card) => card.Item.BasePrice;

        internal static Func<Card, bool> IsCardPlayable = (card) => true;

        internal static Func<EffectData, int, int> CounterEffect = (counterEffect, value) => 0;

        internal static Func<Card, List<Item>, (int multiplier, int bonus)> GetSynergyBonuses
                        = (card, subItems) => (1, 0);

        internal static Func<Coroutine> UpdateDurationEffects = () => null;

    }
}
