using System.Collections.Generic;
using _Game.Enums;
using _Game.Interfaces;
using _Game.Scripts.Interfaces;
using _Game.Systems.BlockSystem;
using _Game.Systems.GridSystem;

namespace _Game.Systems.MatchSystem
{
    public class MatchSystem
    {
        private readonly IGridHandler _grid;
        private readonly IEventBus _eventBus;

        public MatchSystem(IGridHandler grid, IEventBus eventBus)
        {
            _grid = grid;
            _eventBus = eventBus;
            _eventBus.Subscribe<GridSystem.BlockSelectedEvent>(OnBlockSelected);
        }

        private void OnBlockSelected(GridSystem.BlockSelectedEvent e)
        {
            var block = _grid.GetBlock(e.Row, e.Col);
            if (block == null) return;

            var group = DetectGroup(e.Row, e.Col, block.Type);
            if (group.Count >= 2)
                _eventBus.Fire(new GridSystem.MatchFoundEvent(group));
            else
                _eventBus.Fire(new GridSystem.NoMatchFoundEvent(e.Row, e.Col));
        }

        private List<BlockModel> DetectGroup(int startRow, int startCol, BlockType type)
        {
            var visited = new bool[_grid.Rows, _grid.Columns];
            var result = new List<BlockModel>();
            var queue = new Queue<(int row, int col)>();

            queue.Enqueue((startRow, startCol));
            visited[startRow, startCol] = true;

            while (queue.Count > 0)
            {
                var (r, c) = queue.Dequeue();
                var b = _grid.GetBlock(r, c);
                if (b == null || b.Type != type) continue;

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