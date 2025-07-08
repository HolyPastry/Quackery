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
        [SerializeField] private TextMeshProUGUI _numNewCustomersText;

        protected override void SetInfo(NotificationInfo _)
        {

            var clients = ClientServices.GetClients();
            List<Client> activateClients = new();
            List<Client> newClients = new();
            foreach (var client in clients)
            {
                if (client.IsInQueue)
                    activateClients.Add(client);

                if (client.IsNew)
                    newClients.Add(client);
            }
            _numCustomersText.text = $"{activateClients.Count} Customers waiting";
            _numNewCustomersText.text = $"{newClients.Count} New Customers!";

            for (int i = 0; i < _portraitImages.Count; i++)
            {
                if (i < activateClients.Count)
                    _portraitImages[i].Show(activateClients[i].Portrait);
                else
                    _portraitImages[i].Hide();

            }
            _morePortraits.SetActive(activateClients.Count > _portraitImages.Count);
        }
    }


}
