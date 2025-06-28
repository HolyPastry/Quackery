using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.Effects
{


    [CreateAssetMenu(fileName = "BoostPriceEffect", menuName = "Quackery/Effects/ValuePrice")]
    public class ValuePriceEffect : EffectData
    {
        public EnumItemCategory Category;

        public override string GetDescription()
        {
            return Sprites.Replace(Category, Description);
        }


        public override int PriceModifier(Effect effect, Card card)
        {
            if (card.Category == Category || Category == EnumItemCategory.Unset)
            {
                return effect.Value;
            }
            return 0;
        }


    }
}
