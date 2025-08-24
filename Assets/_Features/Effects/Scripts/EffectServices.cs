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
        public static Func<IEffectCarrier, Coroutine> Add = (effectCarrier) => null;
        public static Func<Predicate<Effect>, Coroutine> Remove = (predicate) => null;
        public static Func<Predicate<Effect>, IEnumerable<Effect>> Get = (predicate) => new List<Effect>();
        public static Func<IEffectCarrier, Coroutine> RemoveLinkedToObject = (effectCarrier) => null;

        public static Func<Type, float> GetModifier = (effectData) => 0;

        public static Func<List<Effect>> GetCurrent = () => new();

        internal static Func<EnumEffectTrigger, IEffectCarrier, Coroutine> Execute = (trigger, card) => null;
        public static Func<EnumEffectTrigger, CardPile, Coroutine> ExecutePile = (trigger, cardPile) => null;

        internal static Action CleanEffects = delegate { };

        internal static Func<Status, int, int> CounterEffect = (counterStatus, value) => 0;

        internal static Func<Coroutine> UpdateDurationEffects = () => null;

        internal static Func<Dictionary<Status, int>> GetActiveStatuses = () => new();

        internal static Func<EffectData, IEffectCarrier, Coroutine> AddToCarrier = (effect, target) => null;
    }
}
