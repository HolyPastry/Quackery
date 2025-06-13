using System.Collections.Generic;
using UnityEngine;

namespace Quackery
{
    public class StatusManager : MonoBehaviour
    {
        private readonly List<Status> _statuses = new();

        void OnEnable()
        {
            StatusServices.AddStatus = AddStatus;
            StatusServices.RemoveStatus = RemoveStatus;
            StatusServices.UpdateStatusValue = UpdateStatusValue;
        }
        void OnDisable()
        {
            StatusServices.AddStatus = delegate { };
            StatusServices.RemoveStatus = delegate { };
            StatusServices.UpdateStatusValue = delegate { };
        }

        public void AddStatus(StatusData statusData, Vector2 origin)
        {
            if (_statuses.Exists(s => s.Data == statusData))
            {
                Debug.LogWarning($"Status {statusData.name} already exists.");
                return;
            }
            var status = new Status { Data = statusData, Value = statusData.StartValue };
            _statuses.Add(status);
            StatusEvents.OnStatusAdded?.Invoke(status, origin);
        }

        public void RemoveStatus(StatusData statusData)
        {
            var status = _statuses.Find(s => s.Data == statusData);
            if (status == null)
            {
                Debug.LogWarning($"Status {statusData.name} not found.");
                return;
            }

            _statuses.Remove(status);
            StatusEvents.OnStatusRemoved?.Invoke(statusData);

        }

        public void UpdateStatusValue(StatusData statusData, int value)
        {
            var status = _statuses.Find(s => s.Data == statusData);
            if (status == null)
            {
                Debug.LogWarning($"Status {statusData.name} not found.");
                return;
            }

            status.Value = value;
            StatusEvents.OnStatusUpdated?.Invoke(status);

        }
    }
}
