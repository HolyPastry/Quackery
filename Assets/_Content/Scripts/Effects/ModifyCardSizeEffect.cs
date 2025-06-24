using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{

    [CreateAssetMenu(fileName = "ModifyCardSize", menuName = "Quackery/Effects/ModifyCartSize", order = 0)]
    public class ModifyCardSizeEffect : EffectData
    {
        public override void Execute(Effect effect, CardPile pile)
        {
            DeckServices.ModifyCartSize(effect.Value);
        }

        public override void Cancel(Effect effect)
        {
            DeckServices.ModifyCartSize(-effect.Value);
        }
    }
}
