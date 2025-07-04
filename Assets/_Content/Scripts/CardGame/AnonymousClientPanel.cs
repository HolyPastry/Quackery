using System;
using Quackery.Clients;
using Quackery.Decks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery
{
    public class AnonymousClientPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _hiddable;
        [SerializeField] private Image _portrait;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _cartAmount;
        [SerializeField] private TextMeshProUGUI _resultString;

        public void Show(Client client, bool isSuccess)
        {

            _portrait.sprite = client.Portrait;
            _portrait.color = Color.white;
            _nameText.text = client.LoginName;
            if (isSuccess)
            {
                _resultString.text = Sprites.Replace("Successful Transaction");
                _cartAmount.text = Sprites.Replace("+ #Coin " + CartServices.GetLastCartValue().ToString("C0"));
            }
            else
            {
                _resultString.text = Sprites.Replace("Client left without paying!");
                _cartAmount.text = Sprites.Replace("+ #Coin 0");
            }
            _hiddable.SetActive(true);

        }

        internal void Hide()
        {
            _hiddable.SetActive(false);

        }
    }
}
