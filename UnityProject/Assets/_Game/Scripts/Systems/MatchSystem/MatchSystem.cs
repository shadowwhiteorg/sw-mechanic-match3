// Systems/MatchSystem/MatchSystem.cs
using System.Collections.Generic;
using _Game.Core.Events;
using _Game.Enums;
using _Game.Interfaces;
using _Game.Systems.BlockSystem;
using _Game.Systems.GridSystem;

namespace _Game.Systems.MatchSystem
{
    public class MatchSystem : IUpdatableSystem
    {
        private readonly IGridHandler _grid;
        private readonly IEventBus _events;
        private readonly int _matchThreshold;

        public MatchSystem(IGridHandler grid, IEventBus events, int threshold)
        {
            _grid = grid;
            _events = events;
            _matchThreshold = threshold;
            _events.Subscribe<BlockSelectedEvent>(OnBlockSelected);
        }

        public void Tick() { }

        private void OnBlockSelected(BlockSelectedEvent e)
        {
            if (!_grid.TryGet(e.Row, e.Col, out var block)) return;
            if (block.Type != BlockType.None) return;

            var group = DetectGroup(e.Row, e.Col, block.Color);
            if (group.Count >= _matchThreshold)
                _events.Fire(new MatchFoundEvent(group,e.Row, e.Col));
        }

        private List<BlockModel> DetectGroup(int row, int col, BlockColor color)
        {
            var result = new List<BlockModel>();
            var visited = new bool[_grid.Rows, _grid.Columns];
            var queue = new Queue<(int r, int c)>();
            queue.Enqueue((row, col));
            visited[row, col] = true;

            while (queue.Count > 0)
            {
                var (r, c) = queue.Dequeue();
                var b = _grid.GetBlock(r, c);
                if (b == null || b.Color != color) continue;

                result.Add(b);
                foreach (var (nr, nc) in new[] { (r - 1, c), (r + 1, c), (r, c - 1), (r, c + 1) })
                {
                    if (_grid.IsInside(nr, nc) && !visited[nr, nc])
                    {
                        visited[nr, nc] = true;
                        queue.Enqueue((nr, nc));
                    }
                }
            }

            return result;
        }
    }
}
