using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Game.Core.Events;
using _Game.Enums;
using _Game.Interfaces;
using _Game.systems.BlockSystem;
using _Game.Utils;

namespace _Game.Systems.MatchSystem
{
    public class ClearSystem
    {
        private readonly IGridHandler             _grid;
        private readonly IBlockFactory            _factory;
        private readonly IEventBus                _events;
        private readonly SpecialBlockSpawnConfig  _spawnConfig;
        private readonly List<(int row, int col)> _pending = new();
        private bool                               _batching;

        public ClearSystem(
            IGridHandler            grid,
            IBlockFactory           factory,
            IEventBus               events,
            SpecialBlockSpawnConfig spawnConfig)
        {
            _grid        = grid;
            _factory     = factory;
            _events      = events;
            _spawnConfig = spawnConfig;

            _events.Subscribe<MatchFoundEvent>(   OnMatchFound);
            _events.Subscribe<BlockSelectedEvent>(OnBlockSelected);
            _events.Subscribe<ClearBlockEvent>(   OnClearBlock);
        }
        
        private void OnMatchFound(MatchFoundEvent e)
        {
            CoroutineRunner.Instance.StartCoroutine(ClearMatchAndSpawnSpecial(e));
        }

        private IEnumerator ClearMatchAndSpawnSpecial(MatchFoundEvent e)
        {
            _batching = true;

            foreach (var blk in e.Blocks)
                _events.Fire(new ClearBlockEvent(blk));

            // wait one frame so OnClearBlock has actually removed them from the grid
            yield return null;

            // spawn the special, if any
            var specialType = _spawnConfig.GetTypeForMatch(e.Blocks.Count);
            if (specialType != BlockType.None)
            {
                // here you can choose color; e.TapOrigin gives you the match origin
                var color = BlockColor.None;
                var special = _factory.CreateBlock(color, specialType, e.TouchOrigin.x, e.TouchOrigin.y);
                special.Settle(true); // mark it settled so fall doesn’t clear it
            }

            // now flush exactly once for that whole batch
            FlushPending();

            _batching = false;
        }

        private void OnBlockSelected(BlockSelectedEvent e)
        {
            if (!_grid.TryGet(e.Row, e.Col, out var blk) || blk == null) return;
            blk.Activated();
        }

        private void OnClearBlock(ClearBlockEvent e)
        {
            var blk = e.Block;
            if (!blk.IsSettled) return;

            if (!_grid.TryGet(blk.Row, blk.Column, out var live) || live != blk)
                return;

            // remove & buffer
            _grid.SetBlock(blk.Row, blk.Column, null);
            _factory.RecycleBlock(blk);
            _pending.Add((blk.Row, blk.Column));
            blk.Cleared();

            if (!_batching)
                FlushPending();
        }

        private void FlushPending()
        {
            if (_pending.Count == 0) return;
            _events.Fire(new BlocksClearedEvent(_pending.ToList()));
            _pending.Clear();
        }
    }
}
