using KBCore.Refs;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery.GameMenu.Test
{
    public class AddCashTestButton : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private Button _button;
        [SerializeField] private int _amount = 100;

        // Start is called before the first frame update
        void Start()
        {
            _button.onClick.AddListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            PurseServices.Modify(_amount);
        }
    }
}
