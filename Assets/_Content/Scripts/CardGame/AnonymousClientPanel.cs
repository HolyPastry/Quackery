using System;
using KBCore.Refs;
using Quackery.Clients;
using Quackery.Decks;
using Quackery.Followers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery
{
    public class AnonymousClientPanel : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private AudioSource _audioSource;
        [SerializeField] private GameObject _hiddable;

        [SerializeField] private Image _portrait;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _cartAmount;
        [SerializeField] private TextMeshProUGUI _resultString;
        [SerializeField] private FollowerBadge _followerBadge;

        public void Show(Client client, bool concluded, CartMode cartMode)
        {

            _portrait.sprite = client.Portrait;
            _portrait.color = Color.white;
            _nameText.text = client.LoginName;

            var cartValue = CartServices.GetValue();
            var cartBonus = CartServices.GetBonus();

            string evaluation = cartMode switch
            {
                CartMode.Survival => "Poor",
                CartMode.Normal => "Good",
                CartMode.SuperSaiyan => "Outstanding",
                _ => throw new NotImplementedException()
            };

            int newFollowers = FollowerServices.RewardFollowers(cartMode, concluded);


            // _audioSource.PlayOneShot(evaluation.SoundBite);  

            if (concluded)
            {
                _resultString.text = Sprites.Replace($"{evaluation} Transaction");
                _cartAmount.text = Sprites.Replace($"{cartValue + cartBonus}#Coin");
                StartCoroutine(_followerBadge.CountFollowersUpRoutine(newFollowers));
            }
            else
            {
                _resultString.text = Sprites.Replace("Client left without paying!");
                _cartAmount.text = Sprites.Replace("0#Coin");
                _followerBadge.Hide();
            }
            _hiddable.SetActive(true);

        }

        internal void Hide()
        {
            _hiddable.SetActive(false);

        }
    }
}
