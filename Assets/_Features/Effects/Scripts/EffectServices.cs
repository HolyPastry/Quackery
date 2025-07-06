using System;
using System.Collections.Generic;
using Quackery.Decks;
using Quackery.Effects;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery
{
    public static class EffectServices
    {
        public static Action<Effect> AddStatus = (data) => { };
        public static Action<EffectData> Cancel = delegate { };

        // public static Action<int> RemoveById = delegate { };

        // public static Action<int, int> UpdateValue = delegate { };

        public static Func<List<Effect>> GetCurrent = () => new();

        internal static Func<EnumEffectTrigger, Card, int> Execute = (trigger, card) => 0;
        public static Action<EnumEffectTrigger, CardPile> ExecutePile = (trigger, cardPile) => { };

        internal static Action<List<CardPile>> RemoveEffectsLinkedToPiles = delegate { };


        // internal static Func<Card, int> GetPriceModifier = (card) => 0;
        // internal static Func<Card, int> GetRatingModifier = (card) => 0;

        internal static Action CleanEffects = delegate { };


        public static Func<EffectData, int> GetValue = (effectData) => 1;


        internal static Action<EffectData, int> ModifyValue = (effectData, value) => { };
        internal static Action<EffectData, int> SetValue = (effectData, value) => { };

        internal static Action<List<EffectData>> CancelAllEffects = delegate { };

        internal static Action<EnumItemCategory> ChangePreference = (category) => { };

        internal static Func<Card, int> GetCardPrice = (card) => card.Item.BasePrice;

        internal static Func<Card, List<Item>, int> GetStackPrice = (topCard, subItems) => 0;

        internal static Func<Card, bool> IsCardPlayable = (card) => true;

        internal static Func<EffectData, int, int> CounterEffect = (counterEffect, value) => 0;

        internal static Func<int> GetCartSizeModifier = () => 0;

    }
}
