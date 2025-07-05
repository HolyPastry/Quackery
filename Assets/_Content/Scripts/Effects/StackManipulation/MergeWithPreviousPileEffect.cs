using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "MergeWithPreviousPile", menuName = "Quackery/Effects/MergeWithPreviousPile", order = 0)]
    public class MergeWithPreviousPileEffect : CategoryEffectData
    {

        public enum EnumTargetStack
        {
            Previous,
            LowestValue
        }

        public EnumTargetStack TargetStack;
        public EnumPileLocation Location;


    }
}
