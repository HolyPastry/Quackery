using UnityEngine;
using UnityEngine.UI;


namespace Quackery
{
    public class MaskedImage : MonoBehaviour
    {
        [SerializeField] private Image _image;
        public void Show(Sprite sprite)
        {
            gameObject.SetActive(true);
            _image.sprite = sprite;

        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }


}
