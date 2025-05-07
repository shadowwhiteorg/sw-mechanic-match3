using System.Collections.Generic;
using _Game.Interfaces;
using _Game.Systems.GridSystem;
using _Game.Core.Events;
using _Game.Scripts.Core.Events;

namespace _Game.Systems
{
    public class ClearSystem
    {
        private readonly GridHandler _grid;
        private readonly BlockFactory _factory;
        private readonly IEventBus _eventBus;

        public ClearSystem(
            GridHandler grid,
            BlockFactory factory,
            IEventBus eventBus)
        {
            _grid      = grid;
            _factory   = factory;
            _eventBus  = eventBus;
            _eventBus.Subscribe<MatchFoundEvent>(OnMatchFound);
        }

        private void OnMatchFound(MatchFoundEvent e)
        {
            var cleared = new List<(int row,int col)>();

            // recycle each matched block
            foreach (var blk in e.Blocks)
            {
                cleared.Add((blk.Row, blk.Column));
                _grid.SetBlock(blk.Row, blk.Column, null);
                _factory.RecycleBlock(blk);
            }

            // notify FallSystem
            _eventBus.Fire(new BlocksClearedEvent(cleared));
        }
    }
}