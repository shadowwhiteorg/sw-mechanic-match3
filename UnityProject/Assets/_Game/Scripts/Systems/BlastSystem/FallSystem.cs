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
        private readonly IGridHandler       _grid;
        private readonly GridWorldHelper    _helper;
        private readonly IBlockFactory      _factory;
        private readonly IEventBus          _events;
        private readonly float              _fallSpeed;
        private int                          _activeAnimations;

        public FallSystem(
            IGridHandler    grid,
            GridWorldHelper helper,
            IBlockFactory   factory,
            IEventBus       events,
            float           fallSpeed = 7.5f
        ) {
            _grid      = grid;
            _helper    = helper;
            _factory   = factory;
            _events    = events;
            _fallSpeed = fallSpeed;

            _events.Subscribe<BlocksClearedEvent>(OnBlocksCleared);
        }

        private void OnBlocksCleared(BlocksClearedEvent evt)
        {
            // handle each affected column
            foreach (int col in evt.ClearedPositions.Select(p => p.col).Distinct())
            {
                CoroutineRunner.Instance.StartCoroutine(HandleColumnFall(col));
            }
        }

        private IEnumerator HandleColumnFall(int col)
        {
            // 1) slide existing blocks downward
            var moved = SlideDown(col);
            AnimateSlides(col, moved);

            // 2) wait for all slide tweens to finish
            // yield return new WaitUntil(() => _activeAnimations == 0);
            yield return null;

            // 3) spawn brand-new blocks into the holes
            SpawnNewBlocks(col);
        }

        private List<BlockModel> SlideDown(int col)
        {
            var moved = new List<BlockModel>();

            for (int row = _grid.Rows - 1; row >= 0; row--)
            {
                if (_grid.GetBlock(row, col) != null)
                    continue;

                // find the next block above
                int above = row - 1;
                while (above >= 0 && _grid.GetBlock(above, col) == null)
                    above--;

                if (above < 0)
                    continue;  // no more blocks above → keep looking for other holes

                // move it down
                var blk = _grid.GetBlock(above, col);
                _grid.SetBlock(row,    col, blk);
                _grid.SetBlock(above,  col, null);
                blk.SetGridPosition(row, col);
                moved.Add(blk);
            }

            return moved;
        }

        private void AnimateSlides(int col, List<BlockModel> moved)
        {
            foreach (var blk in moved)
            {
                Vector3 from = blk.View.transform.position;
                Vector3 to   = _helper.GetWorldPosition(blk.Row, blk.Column);
                float   dur  = Vector3.Distance(from, to) / _fallSpeed;

                _activeAnimations++;
                Tween.Position(
                    blk.View.transform,
                    to,
                    dur,
                    Ease.OutQuad,
                    onComplete: () =>
                    {
                        blk.Fell();
                        _activeAnimations--;
                        TryFireSettledEvent();
                    }
                );
            }
        }

        private void SpawnNewBlocks(int col)
        {
            // find which rows are still empty, in ascending order
            var emptyRows = Enumerable
                .Range(0, _grid.Rows)
                .Where(r => _grid.GetBlock(r, col) == null)
                .OrderBy(r => r)
                .ToList();

            int toSpawn = emptyRows.Count;
            // Debug.Log($"[Fall] Column {col} → empties: {string.Join(",", emptyRows)} → toSpawn={toSpawn}");
            if (toSpawn == 0) return;

            // spawn one per empty slot
            for (int i = 0; i < toSpawn; i++)
            {
                // the row we need to fill
                int targetRow   = emptyRows[i];
                // compute a negative spawnRow so blocks start off-grid
                int spawnRow    = i - toSpawn-1;   // yields [-toSpawn, …, -1]
                Vector3 spawnPos = _helper.GetWorldPosition(spawnRow, col);

                // create & place off-grid
                var blk = _factory.CreateRandomBlock(targetRow, col);
                _grid.SetBlock(targetRow, col, blk);
                blk.View.transform.position = spawnPos;
                
                // animate into place
                Vector3 targetPos = _helper.GetWorldPosition(targetRow, col);
                float   dur       = Vector3.Distance(spawnPos, targetPos) / _fallSpeed;

                _activeAnimations++;
                blk.Settle(false);
                Tween.Position(
                    blk.View.transform,
                    targetPos,
                    dur,
                    Ease.OutQuad,
                    onComplete: () =>
                    {
                        blk.Fell();
                        blk.Settle(true);
                        _activeAnimations--;
                        TryFireSettledEvent();
                    }
                );
            }
        }

        private void TryFireSettledEvent()
        {
            if (_activeAnimations <= 0)
                _events.Fire(new TurnEndedEvent());
        }
    }
}
