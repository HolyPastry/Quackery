using UnityEngine;

public static class Services
{
    public static void NotImplemented(string name = "")
    {
        Debug.LogWarning($"Not implemented: {name}");
    }
}
