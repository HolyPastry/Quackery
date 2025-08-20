
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
            Card card = effect.LinkedObject as Card;
            Assert.IsNotNull(card, "ExhaustEffect requires a linked card to execute.");
            DeckServices.MoveCard(card, EnumCardPile.Exhaust, EnumPlacement.OnTop, 0.5f);
            yield return Tempo.WaitForABeat;
            yield return DeckServices.DrawBackToFull();
            yield return Tempo.WaitForABeat;
        }
    }
}
