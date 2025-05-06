using System;
using _Game.Enums;
using UnityEngine;
using _Game.Systems.GridSystem;

namespace _Game.Systems.BlockSystem
{
    /// <summary>
    /// Maps each BlockType to its Sprite.
    /// </summary>
    [CreateAssetMenu(menuName = "Configs/BlockTypeConfig", fileName = "BlockTypeConfig")]
    public class BlockTypeConfig : ScriptableObject
    {
        [Serializable]
        public struct Entry
        {
            public BlockType Type;
            public Sprite Sprite;
        }

        [SerializeField] private Entry[] _entries;

        /// <summary>Fetches the sprite for the given block type.</summary>
        public Sprite GetSprite(BlockType type)
        {
            foreach (var e in _entries)
                if (e.Type == type)
                    return e.Sprite;

            Debug.LogWarning($"[BlockTypeConfig] No sprite for {type}");
            return null;
        }
    }
}