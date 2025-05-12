using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using _Game.Enums;
using _Game.Interfaces;
using _Game.Systems.BehaviorSystem;
using UnityEngine.EventSystems;

namespace _Game.Systems.BlockSystem
{
    [CreateAssetMenu(menuName="Game/Block Color Config", fileName="BlockTypeConfig")]
    public class BlockTypeConfig : ScriptableObject
    {
        public BlockConfigEntry[] Entries;

        private Dictionary<(BlockColor,BlockType),BlockConfigEntry> _map;
        public void BuildLookup()
        {
            _map = Entries
                .ToDictionary(e => (BaseType: e.baseColor,SpecialType: e.type), e => e);
        }

        public BlockConfigEntry Get(BlockColor color, BlockType special)
        {
            if (_map == null) BuildLookup();
            if (_map.TryGetValue((color,special), out var e)) return e;
            // fallback to plain
            return _map[(color, BlockType.None)];
        }
    }

    
    
    [Serializable]
    public struct BlockConfigEntry
    {
        public BlockColor         baseColor;
        public BlockType  type;
        public Sprite            Sprite;
        public IBlockBehavior[]  Behaviors;
        public ParticleSystem  ClearParticlePrefab;
    }

}
