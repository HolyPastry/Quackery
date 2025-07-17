using System.Collections.Generic;
using Quackery.Clients;
using Quackery.Notifications;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Quackery.Notifications
{
    public class ClientNotification : Notification
    {
        [SerializeField] private List<MaskedImage> _portraitImages;
        [SerializeField] private GameObject _morePortraits;
        [SerializeField] private TextMeshProUGUI _numCustomersText;
        [SerializeField] private TextMeshProUGUI _numVIPOnlineText;

        protected override void SetInfo(NotificationInfo _)
        {

            var clientList = ClientServices.GetAllClients();
            List<Client> onlineVIP = new();
            List<Client> clientInQueue = new();
            foreach (var client in clientList)
            {
                if (!client.IsAnonymous && client.IsOnline)
                    onlineVIP.Add(client);

                if (client.IsInQueue)
                    clientInQueue.Add(client);
            }
            if (clientInQueue.Count > 1)
                _numCustomersText.text = $"{clientInQueue.Count} Customers waiting";
            else
                _numCustomersText.text = $"1 Customer waiting";
            if (onlineVIP.Count > 0)
            {
                _numVIPOnlineText.gameObject.SetActive(true);
                _numVIPOnlineText.text = $"{onlineVIP.Count} VIP Members Online!";
            }
            else
            {
                _numVIPOnlineText.gameObject.SetActive(false);
                _numVIPOnlineText.text = "";
            }
            int portraitIndex = 0;
            _portraitImages.ForEach(image => image.Hide());
            foreach (var vipClient in onlineVIP)
            {
                if (portraitIndex < _portraitImages.Count)
                {
                    _portraitImages[portraitIndex].Show(vipClient.Portrait);
                    portraitIndex++;
                }
                else
                    break;
            }
            foreach (var client in clientInQueue)
            {
                if (portraitIndex < _portraitImages.Count)
                {
                    _portraitImages[portraitIndex].Show(client.Portrait);
                    portraitIndex++;
                }
                else
                    break;
            }
            _morePortraits.SetActive(onlineVIP.Count + clientInQueue.Count > _portraitImages.Count);
        }
    }


}
