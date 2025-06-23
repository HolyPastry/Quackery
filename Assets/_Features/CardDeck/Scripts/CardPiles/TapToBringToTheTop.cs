using UnityEngine;
using UnityEngine.EventSystems;


namespace Quackery.Decks
{
    public class TapToBringToTheTop : MonoBehaviour, IPointerUpHandler
    {

        public void OnPointerUp(PointerEventData eventData)
        {
            transform.SetAsLastSibling();
        }
    }
}
