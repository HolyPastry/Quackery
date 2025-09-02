using System.Collections;
using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "Max Card Effect", menuName = "Quackery/Effects/Status/Max Card Effect", order = 1)]
    public class MaxCardEffect : EffectData
    {
        [SerializeField] private ItemData CardData;
        [SerializeField] private EnumCardPile CardPile = EnumCardPile.Hand;

        public override IEnumerator Execute(Effect effect)
        {

            var numCardInHand =
                DeckServices.GetMatchingCards(card => card.Item.Data == CardData, CardPile);

            yield return Tempo.WaitForABeat;
            if (numCardInHand.Count + 1 >= effect.Value) CardGameApp.InterruptRoundRequest();
        }
    }
}
