using System.Collections.Generic;
using _Game.Systems.GridSystem;
using _Game.Interfaces;
using _Game.Systems.BlockSystem;
using UnityEngine;

namespace _Game.Core.Events
{
    public struct BlockSelectedEvent : IGameEvent
    {
        public int Row { get; }
        public int Col { get; }
        public BlockSelectedEvent(int row, int col)
        { 
            (Row, Col) = (row, col);
        }
    }

    public struct MatchFoundEvent : IGameEvent
    {
        public List<BlockModel> Blocks { get; }
        public Vector2Int TouchOrigin { get; }

        public MatchFoundEvent(List<BlockModel> blocks, int row, int col)
        {
            Blocks = blocks;
            TouchOrigin = new Vector2Int(row, col);
            Debug.Log($"MatchFoundEvent: Row:{row} Col:{col} Count:{blocks.Count}");
        }
    }
    

}