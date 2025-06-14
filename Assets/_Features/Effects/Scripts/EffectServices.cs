using System;
using System.Collections.Generic;
using Quackery.Decks;
using Quackery.Effects;
using UnityEngine;

namespace Quackery
{
    public static class EffectServices
    {
        public static Func<Effect, int> Add = (data) => -1;
        public static Action<Effect> Remove = delegate { };
        public static Action<int> RemoveById = delegate { };
        public static Action<int, int> UpdateValue = delegate { };

        public static Func<List<Effect>> GetCurrent = () => new();

        internal static Action<EnumEffectTrigger, CardPile> Execute = (trigger, cardPile) => { };

        internal static Action<List<CardPile>> RemoveEffectsLinkedToPiles = delegate { };


        internal static Func<Card, int> GetPriceModifier = (card) => 0;
        internal static Func<Card, int> GetRatingModifier = (card) => 0;


    }
}
