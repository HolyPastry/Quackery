using System;
using Quackery.Clients;
using Quackery.Decks;
using Quackery.Followers;
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
        [SerializeField] private FollowerBadge _followerBadge;

        public void Show(Client client, bool isSuccess)
        {

            _portrait.sprite = client.Portrait;
            _portrait.color = Color.white;
            _nameText.text = client.LoginName;
            var evaluation = CartServices.GetCartEvaluation();
            var cartValue = CartServices.GetCartValue();
            var cartBonus = CartServices.GetCartBonus();

            int newFollowers = FollowerServices.RewardFollowers(evaluation.Index);

            StartCoroutine(_followerBadge.CountFollowersUpRoutine(newFollowers));

            if (isSuccess)
            {
                _resultString.text = Sprites.Replace($"{evaluation.Description} Transaction");
                _cartAmount.text = Sprites.Replace($"{cartValue + cartBonus}#Coin");
            }
            else
            {
                _resultString.text = Sprites.Replace("Client left without paying!");
                _cartAmount.text = Sprites.Replace("0#Coin");
            }
            _hiddable.SetActive(true);

        }

        internal void Hide()
        {
            _hiddable.SetActive(false);

        }
    }
}
