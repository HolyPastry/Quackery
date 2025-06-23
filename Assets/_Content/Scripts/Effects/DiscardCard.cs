using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "DiscardCard", menuName = "Quackery/Effects/Discard Card", order = 0)]
    public class DiscardCard : EffectData
    {
        // [Tooltip("The amount of cards to discard. If -1, discard all cards in hand.")]
        // [SerializeField] private int _amount = -1;
        public override void Execute(Effect effect, CardPile pile)
        {
            // if (_amount < 0)
            DeckServices.DiscardHand();
            DeckServices.DrawBackToFull();
            // else
            //     DeckServices.DiscardCards(_amount);
        }
    }
}
