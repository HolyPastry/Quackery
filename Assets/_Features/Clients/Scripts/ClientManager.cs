using System.Collections;
using UnityEngine;

namespace Quackery
{


    public class ClientManager : MonoBehaviour
    {
        private ClientList _clientList;

        void Start()
        {
            _clientList = new ClientList();
        }
    }
}
