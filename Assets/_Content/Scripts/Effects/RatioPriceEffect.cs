using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "RatioPriceEffect", menuName = "Quackery/Effects/Ratio Price")]
    public class RatioPriceEffect : EffectData
    {
        public EnumItemCategory Category;
        public float Ratio = 1.0f;

        public override string GetDescription()
        {
            return Sprites.Replace(Category, Description);
        }

        public override void Execute(Effect effect, CardPile pile)
        {
            //noop
        }

        public override float RatioPriceModifier(Effect effect, Card card)
        {
            if (card.Category == Category || Category == EnumItemCategory.Unset)
            {
                return Ratio;
            }
            return 0;
        }

        public override void Cancel(Effect effect)
        { }
    }
}
