using System;
using UnityEngine;
using UnityEngine.Playables;

public static class PurseServices
{
    public static Func<WaitUntil> WaitUntilReady = () => new WaitUntil(() => true);
    public static System.Action<float> Modify = delegate { };
    public static Func<string> GetString = () => string.Empty;
}


public static class PurseEvents
{
    public static System.Action<float> OnPurseUpdated = delegate { };
}