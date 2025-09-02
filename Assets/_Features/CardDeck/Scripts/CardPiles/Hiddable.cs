using UnityEngine;

namespace Quackery
{
    public class Hiddable : MonoBehaviour
    {
        [SerializeField] private GameObject _hiddable;

        void Awake()
        {
            if (_hiddable == null) _hiddable = gameObject;
        }

        public void Show() => _hiddable.SetActive(true);
        public void Hide() => _hiddable.SetActive(false);

        public void SetActive(bool isOn) => _hiddable.SetActive(isOn);


    }
}
