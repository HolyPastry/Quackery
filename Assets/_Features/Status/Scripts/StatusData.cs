using Holypastry.Bakery;
using UnityEngine;

namespace Quackery
{
    [CreateAssetMenu(fileName = "StatusData", menuName = "Quackery/Status/StatusData")]
    public class StatusData : ContentTag
    {
        public Sprite Icon;

        public int StartValue;
        public bool UseValue = false;

        public string Description;
    }
}
