using System;
using System.Collections.Generic;

using Quackery.Inventories;
using UnityEngine;

namespace Quackery
{
    [CreateAssetMenu(fileName = "SpriteLibrary", menuName = "Quackery/Sprite Library")]
    public class SpriteLibrary : ScriptableObject
    {
        [Serializable]
        public record CategorySprite()
        {
            public EnumItemCategory Category;
            public Sprite Icon;
        }

        public List<CategorySprite> Categories;

        public Sprite GetCategoryIcon(EnumItemCategory category)
        {
            foreach (var categorySprite in Categories)
            {
                if (categorySprite.Category == category)
                {
                    return categorySprite.Icon;
                }
            }

            Debug.LogWarning($"No icon found for category: {category}");
            return null;
        }
    }
}
