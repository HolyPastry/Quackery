using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "MergeWithPile", menuName = "Quackery/Effects/Stack/MergeWithPreviousPile", order = 0)]
    public class MergeWithPileEffect : EffectData, ICategoryEffect
    {
        public enum EnumTargetStack
        {
            Unset,
            LowestStack,
            HighestStack,
            LowestValue,
            HighestValue,

        }
        [Tooltip("If true, the card will go in the cart on an empty stack if it cannot match any other stack.\n" +
                 "If false, the card will not be playable if it cannot match any other stack.")]
        public bool AllowEmptyPiles = false;

        [Tooltip("The target stack to merge with. If Previous, it will merge with the last played pile.\n" +
                 "If LowestValue, it will merge with the pile with the lowest value Top Card.\n" +
                 "If SameCategory, it will merge with the pile of the same category.")]
        public EnumTargetStack TargetStack;

        [Tooltip("The location where the card will be placed in the pile.\n" +
                 "OnTop will place the card on top of the pile, while Underneath will place it underneath.")]
        public EnumPlacement Location;
        [SerializeField] private EnumItemCategory _category = EnumItemCategory.Any;
        public EnumItemCategory Category => _category;
    }
}
