using System;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Quackery.Decks
{
    public class BringForward : MonoBehaviour, IPointerDownHandler
    {
        public event Action OnClicked;
        public void OnPointerDown(PointerEventData eventData)
        {
            OnClicked?.Invoke();
        }
    }
}
