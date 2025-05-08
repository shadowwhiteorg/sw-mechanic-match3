// Systems/MatchSystem/ClearSystem.cs
using System.Collections.Generic;
using System.Linq;
using _Game.Core.Events;
using _Game.Enums;
using _Game.Interfaces;
using _Game.Systems.BlockSystem;
using _Game.Systems.GridSystem;

namespace _Game.Systems.MatchSystem
{
    public class ClearSystem
    {
        private readonly IGridHandler _grid;
        private readonly IBlockFactory _factory;
        private readonly IEventBus _events;
        private readonly List<(int, int)> _buffer = new();

        public ClearSystem(IGridHandler grid, IBlockFactory factory, IEventBus events)
        {
            _grid = grid;
            _factory = factory;
            _events = events;

            _events.Subscribe<MatchFoundEvent>(OnMatch);
            _events.Subscribe<ClearBlockEvent>(OnClear);
        }

        private void OnMatch(MatchFoundEvent e)
        {
            foreach (var block in e.Blocks)
            {
                block.Matched();
                _events.Fire(new ClearBlockEvent(block));
            }
        }

        private void OnClear(ClearBlockEvent e)
        {
            var b = e.Block;
            if (_grid.TryGet(b.Row, b.Column, out var live) && live == b)
            {
                _grid.SetBlock(b.Row, b.Column, null);
                _factory.RecycleBlock(b);
                _buffer.Add((b.Row, b.Column));
                b.Cleared();
            }

            if (_buffer.Count > 0)
            {
                _events.Fire(new BlocksClearedEvent(_buffer));
                _buffer.Clear();
            }
        }
    }
}
