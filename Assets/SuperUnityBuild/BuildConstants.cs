using System;

// This file is auto-generated. Do not modify or move this file.

namespace SuperUnityBuild.Generated
{
    public enum ReleaseType
    {
        None,
        Prototype,
    }

    public enum Platform
    {
        None,
        macOS,
        Windows,
        Android,
        iOS,
    }

    public enum ScriptingBackend
    {
        None,
        IL2CPP,
        Mono,
    }

    public enum Target
    {
        None,
        Player,
    }

    public enum Distribution
    {
        None,
    }

    public static class BuildConstants
    {
        public static readonly DateTime buildDate = new DateTime(638899986390620740);
        public const string version = "0.0.10";
        public const int buildCounter = 11;
        public const ReleaseType releaseType = ReleaseType.Prototype;
        public const Platform platform = Platform.iOS;
        public const ScriptingBackend scriptingBackend = ScriptingBackend.IL2CPP;
        public const Target target = Target.Player;
        public const Distribution distribution = Distribution.None;
    }
}

