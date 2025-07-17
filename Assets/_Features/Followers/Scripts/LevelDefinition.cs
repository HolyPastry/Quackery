using System.Numerics;
using UnityEngine;

namespace Quackery.Followers
{
    [System.Serializable]
    public record LevelDefinition
    {
        public Sprite Icon;
        public string Name;
        public int Level;
        public int RequiredFollowers;
        public int RewardScaling;
        public Vector2Int QueueSizeRange;

        public int QueueSize => UnityEngine.Random.Range(QueueSizeRange.x, QueueSizeRange.y + 1);
    }
}
