using System;
using UnityEngine;
using UnityEngine.Playables;

public static class PurseServices
{
    public static Func<WaitUntil> WaitUntilReady = () => new WaitUntil(() => true);
    public static System.Action<float> Modify = delegate { };
    public static Func<string> GetString = () => "<sprite name=Coin>156";

    internal static Func<int, bool> CanAfford = (amount) => true;

    internal static Func<int> GetAmount = () => 156;
}


public static class PurseEvents
{
    public static System.Action<float> OnPurseUpdated = delegate { };
}