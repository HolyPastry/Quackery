using System;
using System.Collections;
using System.Collections.Generic;
using Quackery.Clients;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Quackery
{
    public class ClientPanelUI : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image _portrait;
        [SerializeField] private Image _background;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _chatText;
        [SerializeField] private GameObject _badge;

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
            _badge.SetActive(false);
            _background.color = ColorLibrary.instance.Get("ClientPanelBackgroundAway");
            _nameText.color = ColorLibrary.instance.Get("ClientPanelNameAway");
        }

        private void UpdateUI()
        {

            _badge.SetActive(_client.IsInQueue && !_client.Served);
            _portrait.sprite = _client.Data.Icon;
            _nameText.text = _client.Data.Name;
            _chatText.text = _client.ChatLastLine;

            if (_client.IsInQueue)
            {
                _background.color = ColorLibrary.instance.Get("ClientPanelBackgroundQueue");
                _nameText.color = ColorLibrary.instance.Get("ClientPanelNameQueue");
            }
            else
            {
                _background.color = ColorLibrary.instance.Get("ClientPanelBackgroundAway");
                _nameText.color = ColorLibrary.instance.Get("ClientPanelNameAway");
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnSelected?.Invoke(_client);
        }
    }
}
