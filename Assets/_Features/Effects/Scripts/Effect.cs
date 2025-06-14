using System;

using Quackery.Decks;
using Quackery.Effects;
using UnityEngine;

namespace Quackery
{
    public class Effect
    {
        public EffectData Data;
        public int Value;

        public Card LinkedCard;

        public Vector2 Origin;

        public string Description => Data.Description.Replace("{#Value}", Value.ToString());

        public EnumEffectTrigger Trigger => Data.Trigger;
        public EnumEffectType Type => Data.Type;
        public Sprite Icon => Data.Icon;

        public Effect(EffectData data)
        {
            Data = data;
            Value = data.StartValue;
        }

        internal void Execute(EnumEffectTrigger trigger, CardPile pile)
        {

            if (trigger != Data.Trigger)
                return;
            if (!pile.IsEmpty) LinkedCard = pile.TopCard;
            Data.Execute(this, pile);
        }

        public int RatingModifier(Card card) => Data.RatingModifier(this, card);
        public int PriceModifier(Card card) => Data.PriceModifier(this, card);


    }
}
