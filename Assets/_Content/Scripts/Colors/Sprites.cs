using System;
using Holypastry.Bakery;
using Quackery.Inventories;

namespace Quackery
{
    public class Sprites : Singleton<Sprites>
    {


        internal static Func<string, string> Replace
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
