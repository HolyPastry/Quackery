using System;
using UnityEngine;


namespace Quackery
{
    public static class StatusEvents
    {
        public static Action<Status> OnStatusUpdated = delegate { };

        public static Action<Status, Vector2> OnStatusAdded = delegate { };
        public static Action<StatusData> OnStatusRemoved = delegate { };
    }
}
