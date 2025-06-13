using UnityEngine;

namespace Quackery.Decks
{
    [CreateAssetMenu(
         fileName = "HypnosisPower",
         menuName = "Quackery/Powers/Hypnosis",
         order = 1)]
    public class HypnosisPower : Power
    {
        public override string Description
             => "<b>Hypnosis</b>: <b>Activated</b> - The Client forgets one of their ailments.";

        public override void Execute(CardPile pile)
        {
            ClientServices.ForgetAilment();
        }
    }
}
