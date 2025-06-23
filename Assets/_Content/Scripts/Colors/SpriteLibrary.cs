using System;
using System.Collections.Generic;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery
{
    public class SpriteLibrary : ScriptableObject
    {
        [Serializable]
        public struct CategorySpriteTextInserts
        {
            public EnumItemCategory Category;
            public string StringInsert;
        }
        public List<CategorySpriteTextInserts> TextInserts;



    }
}
