using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Playables;
public static class ListExtensions
{

    public static void AddUniqueRange<T>(this List<T> list, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            if (!list.Contains(item))
            {
                list.Add(item);
            }
        }
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> list)
    {
        T[] elements = list.ToArray();
        for (int i = elements.Length - 1; i >= 0; i--)
        {
            int swapIndex = UnityEngine.Random.Range(0, i + 1);
            yield return elements[swapIndex];
            elements[swapIndex] = elements[i];
        }
    }
    public static List<T> Shuffle<T>(this List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
        return list;

    }

    public static bool DrawOne<T>(this List<T> drawPile, out T card)
    {
        if (drawPile.Count == 0)
        {
            card = default(T);
            return false;
        }

        card = drawPile[0];
        drawPile.RemoveAt(0);
        return true;
    }
    public static List<T> DrawMany<T>(this List<T> drawPile, int numberCards)
    {
        List<T> drawnCards = new List<T>();
        for (int i = 0; i < numberCards && drawPile.Count > 0; i++)
        {
            if (!drawPile.DrawOne(out T card)) break;
            drawnCards.Add(card);
        }
        return drawnCards;
    }
}