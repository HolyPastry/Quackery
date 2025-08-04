using System.Collections;
using KBCore.Refs;
using Quackery.Clients;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery
{
    public class ClientGameUI : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private AnimatedRect _animatable;
        [SerializeField] private Image _clientImage;
        [SerializeField] private Transform _clientImageTransform;
        [SerializeField] private TMPro.TextMeshProUGUI _clientNameText;
        private Client _client;

        internal void Hide(bool instant = false)
        {
            if (instant)
                _animatable.Hide();

            else
                _animatable.SlideOut(Direction.Left);
            // _animatable.SlideOut(Direction.Top, instant: true);
        }

        internal IEnumerator Show(Client client)
        {
            _client = client;
            _clientImage.sprite = client.Portrait;
            _clientNameText.text = client.LoginName;

            if (client.IsAnonymous)
                _clientImageTransform.localScale = Vector3.one * 0.5f;
            else
                _clientImageTransform.localScale = Vector3.one;
            _animatable.Show();
            _animatable.SlideToZero();
            //_animatable.SlideIn(Direction.Bottom);
            yield return _animatable.WaitForAnimation();
        }


    }
}
