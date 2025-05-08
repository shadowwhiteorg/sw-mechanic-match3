using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using _Game.Core.Events;
using _Game.Enums;
using _Game.Interfaces;
using _Game.Systems.BlockSystem;
using _Game.Systems.GridSystem;
using _Game.Utils;

namespace _Game.Systems.MatchSystem
{
    public class FallSystem
    {
        private readonly IGridHandler _grid;
        private readonly GridWorldHelper _helper;
        private readonly IBlockFactory _factory;
        private readonly IEventBus _events;
        private readonly float _fallSpeed = 7.5f;

        private int _activeAnimations;

        public FallSystem(IGridHandler grid, GridWorldHelper helper, IBlockFactory factory, IEventBus events)
        {
            _grid = grid;
            _helper = helper;
            _factory = factory;
            _events = events;
            _events.Subscribe<BlocksClearedEvent>(OnBlocksCleared);
        }

        private void OnBlocksCleared(BlocksClearedEvent e)
        {
            var columns = e.ClearedPositions.Select(p => p.Item2).Distinct().ToList();

            foreach (int col in columns)
            {
                CoroutineRunner.Instance.StartCoroutine(HandleColumnFall(col));
            }
        }

        private IEnumerator HandleColumnFall(int col)
        {
            var moved = SlideDown(col);
            AnimateMoves(moved);

            int toSpawn = CountEmptySlots(col);
            SpawnNewBlocks(col, toSpawn);

            yield break;
        }

        private List<BlockModel> SlideDown(int col)
        {
            var moved = new List<BlockModel>();

            for (int row = _grid.Rows - 1; row >= 0; row--)
            {
                if (_grid.GetBlock(row, col) != null) continue;

                int above = row - 1;
                while (above >= 0 && _grid.GetBlock(above, col) == null)
                    above--;

                if (above < 0) break;

                var block = _grid.GetBlock(above, col);
                _grid.SetBlock(row, col, block);
                _grid.SetBlock(above, col, null);
                block.SetGridPosition(row, col);
                moved.Add(block);
            }

            return moved;
        }

        private void AnimateMoves(List<BlockModel> moved)
        {
            foreach (var block in moved)
            {
                var targetPos = _helper.GetWorldPosition(block.Row, block.Column);
                float duration = Vector3.Distance(block.View.transform.position, targetPos) / _fallSpeed;

                _activeAnimations++;
                Tween.Position(block.View.transform, targetPos, duration, Ease.OutQuad, () =>
                {
                    block.Fell();
                    _activeAnimations--;
                    TryFireSettledEvent();
                });
            }
        }

        private int CountEmptySlots(int col)
        {
            int count = 0;
            for (int row = 0; row < _grid.Rows; row++)
            {
                if (_grid.GetBlock(row, col) == null) count++;
            }
            return count;
        }

        private void SpawnNewBlocks(int col, int count)
        {
            for (int i = 0; i < count; i++)
            {
                int targetRow = i;
                int spawnRow = i - count;

                var spawnPos = _helper.GetWorldPosition(spawnRow, col);
                var block = _factory.CreateRandomBlock(targetRow, col);
                block.View.transform.position = spawnPos;

                var targetPos = _helper.GetWorldPosition(targetRow, col);
                float duration = Vector3.Distance(spawnPos, targetPos) / _fallSpeed;

                _activeAnimations++;
                Tween.Position(block.View.transform, targetPos, duration, Ease.OutQuad, () =>
                {
                    block.Fell();
                    _activeAnimations--;
                    TryFireSettledEvent();
                });
            }
        }

        private void TryFireSettledEvent()
        {
            if (_activeAnimations <= 0)
                _events.Fire(new BlocksSettledEvent());
        }
    }
}
