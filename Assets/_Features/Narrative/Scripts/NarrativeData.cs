using Holypastry.Bakery;
using Quackery.Clients;
using UnityEngine;

namespace Quackery.Narrative
{
    [CreateAssetMenu(fileName = "NarrativePost", menuName = "Quackery/Narrative Post")]
    public class NarrativeData : ContentTag
    {
        [TextArea(3, 10)]
        public string Message;
        public Sprite Banner;

        public ClientData Creator;

    }
}
