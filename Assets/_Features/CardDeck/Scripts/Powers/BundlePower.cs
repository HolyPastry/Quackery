using System.Collections.Generic;
using UnityEngine;

namespace Quackery.Decks
{
    [CreateAssetMenu(
         fileName = "BundlePower",
         menuName = "Quackery/Powers/Bundle",
         order = 1)]
    public class BundlePower : Power
    {
        public int maxNumberOfCards = 3;

        public override string Description => "<b>Bundle:</b> Allows you to bundle up to " + maxNumberOfCards + " cards together.";
        public override void Execute(CardPile owningPile)
        {
            List<CardPile> piles = DeckServices.GetTablePile();

            foreach (var pile in piles)
            {
                if (pile.Category == owningPile.Category)
                {
                    DeckServices.MovePileTo(pile, owningPile);
                    break;
                }
            }

        }
    }
}
