using System;
using UnityEngine;
using UnityEngine.Playables;

public static class PurseServices
{
    public static Func<WaitUntil> WaitUntilReady = () => new WaitUntil(() => true);
    public static System.Action<float> Modify = delegate { };
    public static Func<string> GetString = () => string.Empty;

    internal static Func<int, bool> CanAfford = (amount) => true;

    internal static Func<int> GetAmount = () => 0;
}


public static class PurseEvents
{
    public static System.Action<float> OnPurseUpdated = delegate { };
}