using System;
using System.Text.RegularExpressions;
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
        public static Func<string, EnumItemCategory, string> ReplaceCategory
            => (text, category) => Instance.ReplaceCategoryInString(category, text);
        internal static Func<string, string> Replace
            => (text) => Instance.ReplaceTags(text);

        internal static Sprite GetCategory(EnumItemCategory category)
        {
            return Instance.Library.Categories.Find(x => x.Category == category)?.Icon;
        }

        private string ReplaceTags(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;
            text = text.Replace("\\n", " \n");

            text = Regex.Replace(text, @"#(\w+)", "<sprite name=$1>");

            return text;
        }

        private string ReplaceCategoryInString(EnumItemCategory category, string text)
        {

            string textInsert = $"<sprite name={category.ToString()}>";

            text = text.Replace("#Category", textInsert);

            return ReplaceTags(text);

        }

    }
}
