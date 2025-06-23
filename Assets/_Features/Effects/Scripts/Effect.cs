using System;

using Quackery.Decks;
using Quackery.Effects;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Quackery
{
    public class Effect
    {

        public EffectData Data
        {
            get => _data;
            set
            {
                _data = value;
                if (_data != null)
                {
                    Key = _data.name;
                }
            }
        }
        private EffectData _data;
        public string Key;
        public int Value;

        [NonSerialized]
        public Card LinkedCard;

        [NonSerialized]
        public Vector2 Origin;

        public string Description => Data.Description.Replace("{#Value}", Value.ToString());

        public EnumEffectTrigger Trigger => Data.Trigger;
        public Sprite Icon => Data.Icon;

        public Effect()
        { }

        public Effect(EffectData data, bool initValue)
        {
            Data = data;
            if (initValue)
                Value = Data.StartValue;
        }

        internal void Execute(CardPile pile)
        {
            if (pile != null && !pile.IsEmpty) LinkedCard = pile.TopCard;
            Data.Execute(this, pile);
        }

        public int RatingModifier(Card card) => Data.RatingModifier(this, card);
        public int PriceModifier(Card card) => Data.PriceModifier(this, card);

        internal bool ContainsTag(EnumEffectTag effectTag)
        {
            if (Data == null) return false;
            return Data.Tags.Contains(effectTag);
        }
    }
}
