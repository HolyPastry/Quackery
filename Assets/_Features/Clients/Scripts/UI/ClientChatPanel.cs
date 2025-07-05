using System;
using Quackery.Chats;
using Quackery.Clients;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery
{
    public class ClientChatPanel : MonoBehaviour
    {
        [SerializeField] protected Image _portrait;
        [SerializeField] protected TMPro.TextMeshProUGUI _loginName;
        [SerializeField] protected ChatWindow _chatWindow;

        void Awake()
        {
            _chatWindow.enabled = false;
        }

        public void SetClientInfo(Client client)
        {
            _portrait.sprite = client.Portrait;
            _loginName.text = client.LoginName;
            _chatWindow.SetChatHistory(client.ChatHistory);
        }
        public void EnableChat() => _chatWindow.enabled = true;
        public void DisableChat()
        {
            _chatWindow.ClearChat();
            _chatWindow.enabled = false;
        }

        internal void Show()
        {
            gameObject.SetActive(true);
        }

        internal void Hide()
        {
            gameObject.SetActive(false);
            DisableChat();



        }
    }
}
