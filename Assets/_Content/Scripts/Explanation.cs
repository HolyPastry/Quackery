using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quackery
{
    [CreateAssetMenu(fileName = "Explanation", menuName = "Quackery/Explanation", order = 0)]
    public class Explanation : ScriptableObject
    {

        public string Name;
        [TextArea(2, 10)]
        public string Definition;
        public string ShortDescription => $"<b>{Name}:</b> {Definition}";
    }
}
