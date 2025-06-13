
using System;
using System.Collections.Generic;
using Quackery.Inventories;
using UnityEditor;
using UnityEngine;

namespace Quackery
{

    [CreateAssetMenu(
        fileName = "CategoryColors",
        menuName = "Quackery/CategoryColors",
        order = 1)]
    public class Colors : ScriptableObject
    {
        [Serializable]
        public struct CategoryColor
        {
            public EnumItemCategory Category;
            public Color Color;
        }
        public List<CategoryColor> CategoryColorsList;


    }
}
