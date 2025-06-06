using System;
using Quackery;
using UnityEngine;

public static class AppServices
{
    public static Action<AppData, Vector2> OpenApp = (appData, position) => { };
    public static Action CloseApp = delegate { };

}
