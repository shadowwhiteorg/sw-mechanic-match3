using System.Collections.Generic;
using _Game.Interfaces;
using _Game.Systems.BlockSystem;

namespace _Game.Core.Events
{
    public struct BlocksClearedEvent : IGameEvent
    {
        public IReadOnlyList<(int row, int col)> ClearedPositions { get; }
        public BlocksClearedEvent(IReadOnlyList<(int,int)> positions)
            => ClearedPositions = positions;
        
    }
    public struct ColumnFallCompletedEvent : IGameEvent
    {
        public int Column { get; }
        public ColumnFallCompletedEvent(int column) => Column = column;
    }
    public struct DuckDeliveredEvent : IGameEvent
    {
        public int Column { get; }

        public DuckDeliveredEvent(int column) => Column = column;
    }
    
    public struct ClearBlockEvent : IGameEvent
    {
        public BlockModel Block;
        public ClearBlockEvent(BlockModel block) => Block = block;
    }
    
}