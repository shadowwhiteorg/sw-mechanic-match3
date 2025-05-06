using System.Collections.Generic;
using _Game.Systems.GridSystem;
using _Game.Interfaces;
using _Game.Systems.BlockSystem;
using UnityEngine;

namespace _Game.Systems.GridSystem
{
    // 1) Fired when player taps/clicks a block
    public struct BlockSelectedEvent : IGameEvent
    {
        public int Row { get; }
        public int Col { get; }
        public BlockSelectedEvent(int row, int col)
        { 
            (Row, Col) = (row, col);
        }
    }

    // 2) Fired when a match group of size>=2 is found
    public struct MatchFoundEvent : IGameEvent
    {
        public IReadOnlyList<BlockModel> Blocks { get; }
        public MatchFoundEvent(IReadOnlyList<BlockModel> blocks)
        { 
            Blocks = blocks;
            Debug.Log($"MatchFoundEvent: Blocks:{Blocks.Count}");
        }
    }

    // 3) Fired when no valid match (group size<2)
    public struct NoMatchFoundEvent : IGameEvent
    {
        public int Row { get; }
        public int Col { get; }
        public NoMatchFoundEvent(int row, int col) => (Row, Col) = (row, col);
    }
}