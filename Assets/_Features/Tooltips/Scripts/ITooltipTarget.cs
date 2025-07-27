
using System.Collections.Generic;
using UnityEngine;


namespace Quackery
{
    public interface ITooltipTarget
    {
        public List<Explanation> Explanations { get; }
        public RectTransform RectTransform { get; }
    }
}
