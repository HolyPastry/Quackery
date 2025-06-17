
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Quackery
{


    [CreateAssetMenu(
        fileName = "CategoryColors",
        menuName = "Quackery/ColorLibrary",
        order = 1)]
    public class ColorLibrary : ScriptableObject
    {
        [Serializable]
        public struct ColorEntry
        {
            public string Id;
            public Color Color;
        }

        [SerializeField] private List<ColorEntry> _colors = new();

        public Color Get(string id)
        {
            if (!_colors.Exists(c => c.Id == id))
            {
                Debug.LogWarning($"Color with id {id} not found in ColorLibrary.");
                return default;
            }
            return _colors.Find(c => c.Id == id).Color;
        }

    }
}
