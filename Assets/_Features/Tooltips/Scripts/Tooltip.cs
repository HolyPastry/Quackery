using KBCore.Refs;
using TMPro;
using UnityEngine;

namespace Quackery
{
    public class Tooltip : ValidatedMonoBehaviour
    {
        [SerializeField, Child] private TextMeshProUGUI _textGUI;

        void Awake()
        {
            gameObject.SetActive(false);
        }

        public void Show(string text)
        {
            _textGUI.text = text;
            gameObject.SetActive(true);
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
