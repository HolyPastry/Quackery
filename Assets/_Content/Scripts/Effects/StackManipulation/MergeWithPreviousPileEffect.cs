using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "MergeWithPreviousPile", menuName = "Quackery/Effects/MergeWithPreviousPile", order = 0)]
    public class MergeWithPreviousPileEffect : EffectData
    {

        public enum EnumTargetStack
        {
            Previous,
            LowestValue
        }

        public EnumTargetStack TargetStack;
        public EnumPileLocation Location;
        public EnumItemCategory Category;
        public override string GetDescription()
        {
            return Sprites.Replace(Category, Description);
        }
        public override void Execute(Effect effect, CardPile pile)
        { }
    }
}
