using System;
using System.Collections;
using System.Collections.Generic;
using Quackery.Artifacts;
using Quackery.Decks;
using Quackery.Effects;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery
{
    public static class EffectServices
    {
        public static Action<Effect> AddEffect = (data) => { };
        public static Action<EffectData> Cancel = delegate { };

        // public static Action<int> RemoveById = delegate { };

        // public static Action<int, int> UpdateValue = delegate { };

        public static Func<List<Effect>> GetCurrent = () => new();

        internal static Func<EnumEffectTrigger, Card, IEnumerator> Execute = (trigger, card) => null;
        public static Func<EnumEffectTrigger, CardPile, IEnumerator> ExecutePile = (trigger, cardPile) => null;

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

        internal static Action<ArtifactData> RemoveArtifactEffects = (artifactData) => { };

        internal static Func<Card, List<Item>, (int multiplier, int bonus)> GetSynergyBonuses
                        = (card, subItems) => (1, 0);

        internal static Action<List<Card>> UpdateCardEffects = (topCards) => { };

        internal static Func<Card, IEnumerator> AddEffectsFromCard = (Card) => null;

    }
}
