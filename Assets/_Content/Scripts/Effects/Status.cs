using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "Status", menuName = "Quackery/Status", order = 1)]
    public class Status : ScriptableObject
    {
        public Sprite Icon;
        public string Name;
        public Explanation Explanation;
        public EnumTarget Target;
    }
}