using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Game.Core.Events;
using _Game.Enums;
using _Game.Interfaces;
using _Game.systems.BlockSystem;
using _Game.Utils;
using UnityEngine;

namespace _Game.Systems.MatchSystem
{
    public class ClearSystem
    {
        private readonly IGridHandler            _grid;
        private readonly IBlockFactory           _factory;
        private readonly IEventBus               _events;
        private readonly SpecialBlockSpawnConfig _spawnConfig;

        private readonly List<(int row, int col)> _pending = new();
        private int _activeBlockCount = 0;
        private bool _batching = false;
        private bool _flushScheduled = false;

        public ClearSystem(
            IGridHandler grid,
            IBlockFactory factory,
            IEventBus events,
            SpecialBlockSpawnConfig spawnConfig)
        {
            _grid        = grid;
            _factory     = factory;
            _events      = events;
            _spawnConfig = spawnConfig;

            _events.Subscribe<MatchFoundEvent>(   OnMatchFound);
            _events.Subscribe<BlockSelectedEvent>(OnBlockSelected);
            _events.Subscribe<ClearBlockEvent>(   OnClearBlock);
            _events.Subscribe<BlockActivatedEvent>(@event=>_activeBlockCount++);
            _events.Subscribe<BlockDeactivatedEvent>(@event=>_activeBlockCount--);
        }

        // MatchFound starts a coroutine that clears the match and spawns the special
        private void OnMatchFound(MatchFoundEvent e)
        {
            CoroutineRunner.Instance.StartCoroutine(ClearMatchAndSpawnSpecial(e));
        }

        private IEnumerator ClearMatchAndSpawnSpecial(MatchFoundEvent e)
        {
            _batching = true;

            foreach (var blk in e.Blocks)
                _events.Fire(new ClearBlockEvent(blk));

            yield return null; // wait a frame to finish all clears

            var specialType = _spawnConfig.GetTypeForMatch(e.Blocks.Count);
            if (specialType != BlockType.None)
            {
                var color = BlockColor.None;
                var direction = specialType == BlockType.Rocket ? (BlockDirection)Random.Range(1, 3) : BlockDirection.None;
                
                var special = _factory.CreateBlock(color, specialType,direction, e.TouchOrigin.x, e.TouchOrigin.y);
                special.Settle(true); // protect from falling
            }

            Flush();
            _batching = false;
        }

        // Special blocks like bombs/rockets/ducks trigger here
        private void OnBlockSelected(BlockSelectedEvent e)
        {
            if (!_grid.TryGet(e.Row, e.Col, out var blk) || blk == null)
                return;
            blk.Activated();
        }

        // Called for every block cleared by match or special
        private void OnClearBlock(ClearBlockEvent e)
        {
            var blk = e.Block;
            if(!blk.CanClear()) return;
            if (!blk.IsSettled) return;
            if (!_grid.TryGet(blk.Row, blk.Column, out var live) || live != blk)
                return;

            // Remove from grid and buffer
            _grid.SetBlock(blk.Row, blk.Column, null);
            _factory.RecycleBlock(blk);
            _pending.Add((blk.Row, blk.Column));
            blk.Cleared();

            // If not inside match-batch, flush at end of frame (once)
            if (!_batching && !_flushScheduled)
            {
                _flushScheduled = true;
                // CoroutineRunner.Instance.StartCoroutine(FlushCoroutine());
                Flush();
            }
        }

        private IEnumerator FlushCoroutine()
        {
            yield return new WaitUntil(() => _activeBlockCount == 0);
            _events.Fire(new BlocksClearedEvent(_pending.ToList()));
            if (_pending.Count != 0)
                _pending.Clear();
            _flushScheduled = false;
            
        }

        private void Flush()
        {
            CoroutineRunner.Instance.StartCoroutine(FlushCoroutine());
        }

        // private void FlushPending()
        // {
        //     if (_pending.Count == 0) return;
        //     CoroutineRunner.Instance.StartCoroutine(WaitActiveBlocksAndFlush());
        // }
        //
        // private IEnumerator WaitActiveBlocksAndFlush()
        // {
        //     yield return new WaitUntil(() => _activeBlockCount == 0);
        //     _events.Fire(new BlocksClearedEvent(_pending.ToList()));
        //     _pending.Clear();
        // }
    }
}
