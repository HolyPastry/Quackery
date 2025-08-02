using KBCore.Refs;
using Quackery.GameMenu;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery.GameMenu.Test
{
    public class MoveArtifactInButton : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private Button _button;
        [SerializeField] private RectTransform _artifactIcon;

        // Start is called before the first frame update
        void Start()
        {
            _button.onClick.AddListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            GameMenuController.AddToArtifactRequest?.Invoke(_artifactIcon);
        }
    }
}
