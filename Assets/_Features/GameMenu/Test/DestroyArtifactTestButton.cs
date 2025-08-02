using KBCore.Refs;
using Quackery.Artifacts;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery.GameMenu.Test
{
    public class DestroyArtifactTestButton : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private Button _button;
        [SerializeField] private ArtifactData _artifactData;

        // Start is called before the first frame update
        void Start()
        {
            _button.onClick.AddListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            GameMenuController.RemoveFromArtifactRequest?.Invoke(_artifactData);
        }
    }
}
