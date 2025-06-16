using System;
using Quackery;
using UnityEngine;

public static class AppServices
{
    public static Action<AppData, Vector2> OpenApp = (appData, position) => { };
    public static Action CloseApp = delegate { };

    internal static Action<AppScreen> RegisterAppScreen = delegate { };

    internal static Action<AppScreen> UnregisterAppScreen = delegate { };

    internal static Func<AppData, bool> IsAppSelected = (appData) => false;
}
