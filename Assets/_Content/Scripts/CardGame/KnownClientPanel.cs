using System;
using System.Collections;
using DG.Tweening;
using Quackery.Clients;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery
{
    public class KnownClientPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _hiddable;
        [SerializeField] private GameObject _revealIdentityPanel;
        [SerializeField] private Image _portrait;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _ratingText;
        [SerializeField] private RatingPanel _ratingPanel;
        //. [SerializeField] private FollowerBadge _followerBadge;

        public void Show(Client client, Client revealedClient)
        {
            _hiddable.SetActive(true);

            if (revealedClient != null)
                StartCoroutine(ShowRevealedClient(client, revealedClient));
            else
                StartCoroutine(ShowClient(client));
        }

        private IEnumerator ShowClient(Client client)
        {
            yield return null;
            _revealIdentityPanel.SetActive(false);
            _portrait.sprite = client.Portrait;
            _portrait.color = Color.white;
            _nameText.text = client.LoginName;
            _ratingText.text = client.LastReviewText;

            yield return StartCoroutine(_ratingPanel.SetRatingRoutine(client.LastRating));
            //      yield return StartCoroutine(_followerBadge.CountFollowersUpRoutine(client.LastFollowersBonus));

        }

        private IEnumerator ShowRevealedClient(Client client, Client revealedClient)
        {
            _portrait.sprite = client.Portrait;
            _portrait.color = Color.white;
            _nameText.text = client.LoginName;
            _ratingText.text = "";
            _ratingPanel.gameObject.SetActive(false);
            ClientServices.SwapClients();

            yield return new WaitForSeconds(0.5f);
            _revealIdentityPanel.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            _portrait.transform.DOScaleX(0, 1).SetEase(Ease.InOutSine);
            _nameText.transform.DOScaleX(0, 1).SetEase(Ease.InOutSine);
            yield return new WaitForSeconds(1f);
            _portrait.sprite = revealedClient.Portrait;
            _nameText.text = revealedClient.LoginName;
            _nameText.transform.DOScaleX(1, 1).SetEase(Ease.InOutSine);
            _portrait.transform.DOScaleX(1, 1).SetEase(Ease.InOutSine);

            yield return new WaitForSeconds(1f);
            _ratingText.text = revealedClient.LastReviewText;
            yield return new WaitForSeconds(1f);
            _ratingPanel.gameObject.SetActive(true);
            yield return StartCoroutine(_ratingPanel.SetRatingRoutine(revealedClient.LastRating));
            //  yield return StartCoroutine(_followerBadge.CountFollowersUpRoutine(revealedClient.LastFollowersBonus));

        }

        public void Hide()
        {
            _hiddable.SetActive(false);
            _revealIdentityPanel.SetActive(false);
            _portrait.sprite = null;
            _portrait.color = Color.clear;
            _nameText.text = string.Empty;
            _ratingText.text = string.Empty;
            _ratingPanel.SetEmpty();
            //   _followerBadge.Hide();
        }
    }
}
