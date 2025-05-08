using System.Collections.Generic;
using _Game.Enums;
using UnityEngine;

namespace _Game.Systems.BlockSystem
{
    [CreateAssetMenu(menuName = "Configs/SpecialBlockSpawnConfig")]
    public class SpecialBlockSpawnConfig : ScriptableObject
    {
        public List<SpecialBlockSpawnRule> rules;

        public BlockType GetTypeForMatch(int size)
        {
            rules.Sort((a, b) => b.matchSize.CompareTo(a.matchSize));
            foreach (var rule in rules)
                if (size >= rule.matchSize)
                    return rule.type;
            return BlockType.None;
        }
    }
    
    [System.Serializable]
    public class SpecialBlockSpawnRule
    {
        public int matchSize;
        public BlockType type;
    }
}
