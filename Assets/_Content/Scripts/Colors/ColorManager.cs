
using System;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery
{
    public class ColorManager : MonoBehaviour
    {
        [SerializeField] private Colors _colors;
        public static Func<EnumItemCategory, Color> Category = (category) => default(Color);
        // Start is called before the first frame update
        void OnEnable()
        {
            Category = GetCategoryColor;
        }

        void OnDisable()
        {
            Category = (category) => default(Color);
        }
        public Color GetCategoryColor(EnumItemCategory category)
        {
            var color = _colors.CategoryColorsList.Find(c => c.Category == category);

            return color.Color;
        }

    }
}
