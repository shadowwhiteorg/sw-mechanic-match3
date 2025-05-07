// Systems/FallSystem.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using _Game.Interfaces;
using _Game.Systems.GridSystem;
using _Game.Core.Events;
using _Game.Utils;
using _Game.Enums;
using _Game.Scripts.Core.Events;
using _Game.Systems.BlockSystem;

namespace _Game.Systems
{
    public class FallSystem
    {
        private readonly GridHandler _grid;
        private readonly GridWorldHelper _helper;
        private readonly BlockFactory _factory;
        private readonly IEventBus _eventBus;
        private readonly GridConfig _gridConfig;
        private readonly float _fallSpeed = 4f;

        public FallSystem(
            GridHandler grid,
            GridWorldHelper helper,
            BlockFactory factory,
            IEventBus eventBus,
            GridConfig gridConfig)
        {
            _grid = grid;
            _helper = helper;
            _factory = factory;
            _eventBus = eventBus;
            _eventBus.Subscribe<BlocksClearedEvent>(OnBlocksCleared);
            _gridConfig = gridConfig;
        }

        private void OnBlocksCleared(BlocksClearedEvent e)
        {
            // for each distinct column that had clears
            foreach (int col in e.ClearedPositions.Select(p => p.col).Distinct())
                HandleColumnFall(col);
        }

        public void HandleColumnFall(int col)
        {
            // 1) Slide down all existing blocks
            var moved = SlideExistingBlocks(col);

            // 2) Animate those slides
            AnimateBlockSlides(moved);

            // 3) Figure out how many new blocks we need
            int toSpawn = ComputeSpawnCount(col);

            // 4) Spawn & animate new blocks
            SpawnNewBlocks(col, toSpawn);
        }

        // 1) Pull any block above each empty down into it
        private List<BlockModel> SlideExistingBlocks(int col)
        {
            var moved = new List<BlockModel>();
            int rows = _grid.Rows;

            for (int row = rows - 1; row >= 0; row--)
            {
                if (_grid.GetBlock(row, col) != null) continue;

                // find first non-null above
                int above = row - 1;
                while (above >= 0 && _grid.GetBlock(above, col) == null)
                    above--;

                if (above >= 0)
                {
                    var blk = _grid.GetBlock(above, col);
                    _grid.SetBlock(row, col, blk);
                    _grid.SetBlock(above, col, null);
                    blk.SetGridPosition(row, col);
                    moved.Add(blk);
                }
            }

            return moved;
        }

        // 2) Tween each moved block into its new world-position
        private void AnimateBlockSlides(List<BlockModel> moved)
        {
            foreach (var blk in moved)
            {
                Vector3 target = _helper.GetWorldPosition(blk.Row, blk.Column);
                float duration = Vector3.Distance(blk.View.transform.position, target) / _fallSpeed;
                Tween.Position(blk.View.transform, target, duration, Ease.OutQuad);
            }
        }

        // 3) Count how many empty slots remain (always at the top once slides are done)
        private int ComputeSpawnCount(int col)
        {
            int filled = 0, rows = _grid.Rows;
            for (int r = 0; r < rows; r++)
                if (_grid.GetBlock(r, col) != null)
                    filled++;
            return rows - filled;
        }

        // 4) Spawn exactly `count` new blocks above the grid and tween them down
        private void SpawnNewBlocks(int col, int count)
        {
            int rows = _grid.Rows;

            for (int i = 0; i < count; i++)
            {
                int targetRow = i; // 0 → top empty, 1 → next, etc.
                int spawnRow = targetRow - count; // negative rows → off-grid above

                Vector3 spawnPos = _helper.GetWorldPosition(spawnRow, col);
                var blk = _factory.CreateRandomBlock(targetRow, col, spawnPos);
                _grid.SetBlock(targetRow, col, blk);

                Vector3 worldTarget = _helper.GetWorldPosition(targetRow, col);
                float duration = Vector3.Distance(spawnPos, worldTarget) / _fallSpeed;
                Tween.Position(blk.View.transform, worldTarget, duration, Ease.OutQuad);
            }
        }
    }
}
