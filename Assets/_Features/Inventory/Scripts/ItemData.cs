using Holypastry.Bakery;
using UnityEngine;

namespace Quackery.Inventories
{
    [CreateAssetMenu(
        fileName = "ItemData",
        menuName = "Quackery/ItemData",
        order = 1)]
    public class ItemData : ContentTag
    {
        public Sprite Icon;
    }
}