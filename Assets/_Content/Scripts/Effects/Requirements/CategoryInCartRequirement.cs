using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.Effects
{

    [CreateAssetMenu(fileName = "CategoryInCartRequirement", menuName = "Quackery/Effects/Category In Cart Requirement", order = 0)]
    public class CategoryInCartRequirement : RequirementEffectData
    {
        [SerializeField] private EnumItemCategory _category = EnumItemCategory.Unset;
        public EnumItemCategory Category => _category;

        public override bool IsFulfilled(Effect effect, Card card)
        {
            var categories = CartServices.GetCategoriesInCart();
            return categories.Contains(_category) && categories.Count == 1;
        }

    }
}
