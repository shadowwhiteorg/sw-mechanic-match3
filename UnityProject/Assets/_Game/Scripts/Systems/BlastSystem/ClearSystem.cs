// Systems/MatchSystem/ClearSystem.cs

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Game.Core.Events;
using _Game.Enums;
using _Game.Interfaces;
using _Game.Systems.BlockSystem;
using _Game.Systems.GridSystem;
using Unity.VisualScripting;

namespace _Game.Systems.MatchSystem
{
    /// <summary>
    /// Handles both group‐clear (matches) and tap‐activation (special blocks),
    /// with a safety guard so blocks mid‐fall cannot be cleared until they’ve landed.
    /// </summary>
    public class ClearSystem
    {
        private readonly IGridHandler           _grid;
        private readonly IBlockFactory          _factory;
        private readonly IEventBus              _events;
        private readonly List<(int row, int col)> _pending = new();

        public ClearSystem(
            IGridHandler   grid,
            IBlockFactory  factory,
            IEventBus      events)
        {
            _grid    = grid;
            _factory = factory;
            _events  = events;

            _events.Subscribe<MatchFoundEvent>(   OnMatchFound);
            _events.Subscribe<BlockSelectedEvent>(OnBlockSelected);
            _events.Subscribe<ClearBlockEvent>(   OnClearBlock);
        }

        private void OnMatchFound(MatchFoundEvent e)
        {
            CoroutineRunner.instance.StartCoroutine(WaitAndClearBlocks(e));
        }

        

        private void OnBlockSelected(BlockSelectedEvent e)
        {
            if (!_grid.TryGet(e.Row, e.Col, out var blk) || blk == null)
                return;
            blk.Activated();
        }

        private void OnClearBlock(ClearBlockEvent e)
        {
            var blk = e.Block;
            // ◀── Safety check: ignore any clear request while the block is still falling
            if (!blk.IsSettled)
                return;

            // ensure it’s still in the grid at that spot
            if (!_grid.TryGet(blk.Row, blk.Column, out var live) || live != blk)
                return;

            RemoveFromGrid(blk);
        }

        public void RemoveFromGrid(BlockModel blk)
        {
            // 1) remove from grid
            _grid.SetBlock(blk.Row, blk.Column, null);
            // 2) recycle its view
            _factory.RecycleBlock(blk);
            // 3) buffer for fall
            _pending.Add((blk.Row, blk.Column));
            // 4) run VFX/SFX hooks
            blk.Cleared();

            // 5) flush once per batch
            if (_pending.Count > 0)
            {
                _events.Fire(new BlocksClearedEvent(_pending.ToList()));
                _pending.Clear();
            }
        }
        
        private IEnumerator WaitAndClearBlocks(MatchFoundEvent e)
        {
            yield return null;
            foreach (var blk in e.Blocks)
                _events.Fire(new ClearBlockEvent(blk));

        }
    }
}
