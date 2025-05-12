using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using _Game.Core.Events;
using _Game.Enums;
using _Game.Interfaces;
using _Game.Systems.BlockSystem;
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
        private int _activeAnimations;
        private bool _canSpawnNewBlocks = true;

        public FallSystem(
            IGridHandler    grid,
            GridWorldHelper helper,
            IBlockFactory   factory,
            IEventBus       events,
            float           fallSpeed = 15f
        ) {
            _grid      = grid;
            _helper    = helper;
            _factory   = factory;
            _events    = events;
            _fallSpeed = fallSpeed;
            _events.Subscribe<LevelCompleteEvent>(e => ActivateDeactivate(false));
            _events.Subscribe<GameOverEvent>(e => ActivateDeactivate(false));
            _events.Subscribe<LevelInitializedEvent>(e => ActivateDeactivate(true));
            _events.Subscribe<BlocksClearedEvent>(OnBlocksCleared);
            
        }

        private void OnBlocksCleared(BlocksClearedEvent evt)
        {
            if (!_canSpawnNewBlocks) return;
            // handle each affected column
            foreach (int col in evt.ClearedPositions.Select(p => p.col).Distinct())
            {
                CoroutineRunner.Instance.StartCoroutine(HandleColumnFall(col));
            }
        }

        private IEnumerator HandleColumnFall(int col)
        {
            var moved = SlideDown(col);
            AnimateSlides(col, moved);
            yield return null;
            SpawnNewBlocks(col);
        }

        private List<BlockModel> SlideDown(int col)
        {
            var moved = new List<BlockModel>();

            for (int row = _grid.Rows - 1; row >= 0; row--)
            {
                if (_grid.GetBlock(row, col) != null)
                    continue;

                int above = row - 1;
                while (above >= 0 && _grid.GetBlock(above, col) == null)
                    above--;

                if (above < 0)
                    continue;

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
            var emptyRows = Enumerable
                .Range(0, _grid.Rows)
                .Where(r => _grid.GetBlock(r, col) == null)
                .OrderBy(r => r)
                .ToList();

            int toSpawn = emptyRows.Count;
            if (toSpawn == 0) return;

            for (int i = 0; i < toSpawn; i++)
            {
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

        private void ActivateDeactivate(bool active)
        {
            _canSpawnNewBlocks = active;
        }

        private void TryFireSettledEvent()
        {
            CoroutineRunner.Instance.StartCoroutine(WaitAndProceed());

            IEnumerator WaitAndProceed()
            {
                yield return new WaitForEndOfFrame();
                if (_activeAnimations <= 0)
                    _events.Fire(new TurnEndedEvent());
            }
        }
    }
}
