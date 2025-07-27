using Quackery.Decks;
using UnityEngine;


namespace Quackery
{
    public class BudgetCartTooltipExtension : TooltipExtension
    {
        public override void SetTooltip(GameObject hoveredObject)
        {
            if (!hoveredObject.TryGetComponent<BudgetCartValueUI>(out BudgetCartValueUI budgetCartValueUI))
                return;

            // _tooltipManager.AddTooltip("Budget Cart",
            //     "The Client will leave if the value of the cart exceed the budget.", new());

        }
    }
}
