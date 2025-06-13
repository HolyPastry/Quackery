using System;
using System.Collections.Generic;
using UnityEngine;

namespace Quackery
{
    public static class StatusServices
    {
        public static Action<StatusData, Vector2> AddStatus = (data, origin) => { };
        public static Action<StatusData> RemoveStatus = delegate { };
        public static Action<StatusData, int> UpdateStatusValue = delegate { };

        public static Func<List<Status>> GetCurrentStatuses = () => new List<Status>();
    }
}
