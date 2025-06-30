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
    }
}
