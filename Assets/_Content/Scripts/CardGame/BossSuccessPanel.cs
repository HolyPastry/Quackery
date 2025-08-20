using System;
using System.Collections;
using Quackery.Clients;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Quackery
{
    internal class BossSuccessPanel : MonoBehaviour
    {
        [SerializeField] private Image _portrait;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _ratingText;
        [SerializeField] private RatingPanel _ratingPanel;
        [SerializeField] private GameObject _successText;
        [SerializeField] private TextMeshProUGUI _rewardText;

        [SerializeField] private GameObject _hiddable;
        internal void Hide()
        {
            _hiddable.SetActive(false);
            _successText.SetActive(false);
            _rewardText.gameObject.SetActive(false);
        }

        internal void Show(Client client)
        {
            StartCoroutine(ShowRoutine(client));
        }

        private IEnumerator ShowRoutine(Client client)
        {
            _hiddable.SetActive(true);
            yield return null;
            _portrait.sprite = client.Portrait;
            _portrait.color = Color.white;
            _nameText.text = client.LoginName;

            _ratingText.text = client.LastReviewText;

            yield return StartCoroutine(_ratingPanel.SetRatingRoutine(client.LastRating));
            yield return Tempo.WaitForABeat;
            _successText.SetActive(true);
            yield return new WaitForSeconds(1f);
            _rewardText.gameObject.SetActive(true);


        }
    }
}
