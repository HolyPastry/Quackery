using System;
using System.Collections.Generic;

using UnityEngine;

namespace Quackery
{
    public class StatusBarUI : MonoBehaviour
    {

        private readonly List<StatusUI> _statusUIPool = new();

        void Awake()
        {
            GetComponentsInChildren(_statusUIPool);
            for (int i = 0; i < _statusUIPool.Count; i++)
            {
                _statusUIPool[i].gameObject.SetActive(false);
            }
        }

        void OnEnable()
        {
            StatusEvents.OnStatusUpdated += UpdateStatusUI;
            StatusEvents.OnStatusAdded += AddStatusUI;
            StatusEvents.OnStatusRemoved += RemoveStatusUI;
        }

        void OnDisable()
        {
            StatusEvents.OnStatusUpdated -= UpdateStatusUI;
            StatusEvents.OnStatusAdded -= AddStatusUI;
            StatusEvents.OnStatusRemoved -= RemoveStatusUI;
        }

        private void RemoveStatusUI(StatusData data)
        {
            for (int i = 0; i < _statusUIPool.Count; i++)
            {
                if (_statusUIPool[i].Status.Data == data)
                {
                    _statusUIPool[i].gameObject.SetActive(false);
                    return;
                }
            }
            Debug.LogWarning($"StatusUI for {data.name} not found in pool.");
        }

        private void AddStatusUI(Status status, Vector2 originPosition)
        {
            foreach (var statusUI in _statusUIPool)
            {
                if (statusUI.gameObject.activeSelf) continue;
                statusUI.gameObject.SetActive(true);
                statusUI.UpdateStatus(status, originPosition);

                return;
            }
        }



        private void UpdateStatusUI(Status status)
        {
            List<Status> statuses = StatusServices.GetCurrentStatuses();

            for (int i = 0; i < statuses.Count; i++)
            {
                var statusUI = _statusUIPool.Find(ui
                        => ui.Status.Data == statuses[i].Data &&
                            ui.gameObject.activeSelf == true
                            );
                if (statusUI != null)
                {
                    statusUI.UpdateStatus(statuses[i]);
                    continue;
                }

            }
            for (int i = 0; i < _statusUIPool.Count; i++)
            {
                if (_statusUIPool[i].gameObject.activeSelf == false ||
                   statuses.Exists(s => s.Data == _statusUIPool[i].Status.Data))
                    continue;

                _statusUIPool[i].gameObject.SetActive(false);
            }
        }


    }
}
