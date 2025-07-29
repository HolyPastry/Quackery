using System;
using UnityEngine;

namespace Quackery.Followers
{
    [Serializable]
    public struct FollowerLevel
    {
        public int Level;
        public int FollowerRequirement;
        public Sprite Icon;
        public string Name;
    }
}
