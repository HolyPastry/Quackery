
using System;
using System.Collections;
using System.Collections.Generic;
using Quackery.Decks;
using Quackery.Effects;
using UnityEngine;
using UnityEngine.Assertions;

namespace Quackery
{


    [CreateAssetMenu(fileName = "ExhaustEffect", menuName = "Quackery/Effects/Deck/Exhaust", order = 0)]
    public class ExhaustEffect : EffectData
    {
        public override IEnumerator Execute(Effect effect)
        {
            Assert.IsNotNull(effect.LinkedCard, "ExhaustEffect requires a linked card to execute.");
            DeckServices.MoveCard(effect.LinkedCard, EnumCardPile.Exhaust, EnumPlacement.OnTop, 1f);
            yield return DefaultWaitTime;
        }
    }
}
