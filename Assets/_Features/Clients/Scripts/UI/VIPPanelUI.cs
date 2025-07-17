using System;
using System.Collections;
using System.Collections.Generic;
using Holypastry.Bakery.Quests;
using Quackery.Clients;
using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Quackery
{
    public class VIPPanelUI : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image _portrait;
        [SerializeField] private Image _background;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _questText;
        [SerializeField] private Image _questBackground;
        [SerializeField] private GameObject _questLinePanel;
        [SerializeField] private VerticalLayoutGroup _rowLayoutGroup;
        [SerializeField] private GameObject _onlineBadge;
        [SerializeField] private GameObject _offlineBadge;
        [SerializeField] private GameObject _newBadge;

        [SerializeField] private EffectBarUI _effectBarUI;

        public Client Client
        {
            get => _client;
            set
            {
                _client = value;
                UpdateUI();
            }
        }

        public Action<Client> OnSelected = delegate { };

        private Client _client;

        void Awake()
        {
            _offlineBadge.SetActive(false);
            _onlineBadge.SetActive(false);
            _newBadge.SetActive(false);
            _background.color = Colors.Get("ClientPanelBackgroundAway");
            _nameText.color = Colors.Get("ClientPanelNameAway");
        }

        private void UpdateUI()
        {
            _newBadge.SetActive(_client.IsNew);
            _onlineBadge.SetActive(_client.IsOnline);
            _offlineBadge.SetActive(!_client.IsOnline);
            _portrait.sprite = _client.Portrait;
            _nameText.text = _client.LoginName;
            SetQuestInfo();

            if (!_client.IsNew)
                SetEffectBar();

            if (_client.IsOnline)
            {
                _background.color = Colors.Get("ClientPanelBackgroundQueue");
                _nameText.color = Colors.Get("ClientPanelNameQueue");
            }
            else
            {
                _background.color = Colors.Get("ClientPanelBackgroundAway");
                _nameText.color = Colors.Get("ClientPanelNameAway");
            }
            _rowLayoutGroup.enabled = false;
            _rowLayoutGroup.enabled = true;
            LayoutRebuilder.ForceRebuildLayoutImmediate(_rowLayoutGroup.transform as RectTransform);
        }

        private void SetEffectBar()
        {
            foreach (var effect in _client.Effects)
                _effectBarUI.AddStatusUI(effect);
        }

        private void SetQuestInfo()
        {
            _questText.text = "";
            _questLinePanel.SetActive(false);

            if (Client.State != Client.EnumState.Revealed &&
                Client.State != Client.EnumState.Ready) return;

            if (_client.State == Client.EnumState.Revealed)
                _questBackground.color = Color.green;

            _questText.text = _client.RevealCondition?.ToString();
            _questLinePanel.SetActive(true);

        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_client.IsOnline)
                OnSelected?.Invoke(_client);
        }
    }
}
