using System;
using Holypastry.Bakery;
using Quackery.Inventories;

namespace Quackery
{
    public class Sprites : Singleton<Sprites>
    {
        //private SpriteLibrary _spriteLibrary;

        public static Func<EnumItemCategory, string, string> Replace
                => (category, text) => Instance.ReplaceCategoryInString(category, text);

        internal static Func<string, string> ReplaceCategories
            => (text) => Instance.ReplaceCategoriesInString(text);

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
