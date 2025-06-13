using Unity.VisualScripting;
using UnityEngine;

namespace Quackery.Decks
{
    [CreateAssetMenu(
         fileName = "HypnosisPower",
         menuName = "Quackery/Powers/SetStatusPower",
         order = 1)]
    public class SetStatusPower : Power
    {
        public override string Description
             => StatusData.Description;
        public StatusData StatusData;
        public override void Execute(CardPile pile)
        {
            StatusServices.AddStatus(StatusData, pile.TopCard.transform.position);
        }
    }
}
