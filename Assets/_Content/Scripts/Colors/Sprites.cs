using System;
using Holypastry.Bakery;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery
{
    public class Sprites : Singleton<Sprites>
    {
        private SpriteLibrary _library;

        public SpriteLibrary Library
        {
            get
            {
                if (_library == null)
                    _library = Resources.Load<SpriteLibrary>("SpriteLibrary");
                if (_library == null)
                    throw new NullReferenceException("SpriteLibrary not found in Resources. Please create a SpriteLibrary");

                return _library;
            }
        }

        internal static Func<string, string> Replace
            => (text) => Instance.ReplaceCategoriesInString(text);

        internal static Sprite GetCategory(EnumItemCategory category)
        {
            return Instance.Library.Categories.Find(x => x.Category == category)?.Icon;
        }

        private string ReplaceCategoriesInString(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;
            foreach (EnumItemCategory category in Enum.GetValues(typeof(EnumItemCategory)))
            {

                string textInsert = $"<sprite name={category.ToString()}>";
                text = text.Replace($"#{category.ToString()}", textInsert);
            }
            text = text.Replace("#Coin", "<sprite name=Coin>");
            text = text.Replace("#Rating", "<sprite name=Rating>");

            return text;
        }

        private string ReplaceCategoryInString(EnumItemCategory category, string text)
        {

            string textInsert = $"<sprite name={category.ToString()}>";

            text = text.Replace("#Category", textInsert);

            return text;

        }

    }
}
