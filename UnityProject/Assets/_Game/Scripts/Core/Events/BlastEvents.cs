using System.Collections.Generic;
using _Game.Interfaces;

namespace _Game.Scripts.Core.Events
{
    public struct BlocksClearedEvent : IGameEvent
    {
        public IReadOnlyList<(int row, int col)> ClearedPositions { get; }
        public BlocksClearedEvent(IReadOnlyList<(int,int)> positions)
            => ClearedPositions = positions;
    }
}