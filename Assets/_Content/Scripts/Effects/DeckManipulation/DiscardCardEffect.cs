using System.Collections;
using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{

    [CreateAssetMenu(fileName = "DiscardCard", menuName = "Quackery/Effects/Deck/Discard Card", order = 0)]
    public class DiscardCardEffect : EffectData
    {
        //[Tooltip("The amount of cards to discard. If -1, discard all cards in hand.")]
        // [SerializeField] private int _amount = -1;
        public override IEnumerator Execute(Effect effect)
        {
            DeckServices.DiscardCards(effect.Value);
            yield return DefaultWaitTime;
        }
    }
}
