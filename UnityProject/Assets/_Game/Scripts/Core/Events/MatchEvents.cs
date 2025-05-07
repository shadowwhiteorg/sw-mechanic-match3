using System.Collections.Generic;
using _Game.Systems.GridSystem;
using _Game.Interfaces;
using _Game.Systems.BlockSystem;
using UnityEngine;

namespace _Game.Systems.GridSystem
{
    public struct BlockSelectedEvent : IGameEvent
    {
        public int Row { get; }
        public int Col { get; }
        public BlockSelectedEvent(int row, int col)
        { 
            (Row, Col) = (row, col);
            // Debug.Log($"BlockSelectedEvent: Row:{Row} Col:{Col}");
        }
    }

    public struct MatchFoundEvent : IGameEvent
    {
        public IReadOnlyList<BlockModel> Blocks { get; }
        public MatchFoundEvent(IReadOnlyList<BlockModel> blocks)
        { 
            Blocks = blocks;
            // Debug.Log($"MatchFoundEvent: Blocks:{Blocks.Count}");
        }
    }

    public struct NoMatchFoundEvent : IGameEvent
    {
        public int Row { get; }
        public int Col { get; }
        public NoMatchFoundEvent(int row, int col) => (Row, Col) = (row, col);
    }
}