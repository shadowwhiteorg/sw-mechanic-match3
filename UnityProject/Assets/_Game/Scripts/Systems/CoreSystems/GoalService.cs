using _Game.Systems.BlockSystem;
using System.Collections.Generic;
using _Game.Interfaces;
using _Game.Core.Events;
using _Game.Systems.GridSystem;

namespace _Game.Systems.CoreSystems
{
    public class GoalService
    {
        private readonly GridHandler _grid;
        private readonly BlockFactory _factory;
        private readonly IEventBus _eventBus;

        private readonly List<BlockModel> _ducks = new();

        public GoalService(GridHandler grid, BlockFactory factory, IEventBus eventBus)
        {
            _grid = grid;
            _factory = factory;
            _eventBus = eventBus;

            _eventBus.Subscribe<ColumnFallCompletedEvent>(OnColumnFallCompleted);
        }

        public void RegisterDuck(BlockModel duck)
        {
            if (!_ducks.Contains(duck))
                _ducks.Add(duck);
        }

        private void OnColumnFallCompleted(ColumnFallCompletedEvent e)
        {
            for (int i = _ducks.Count - 1; i >= 0; i--)
            {
                var duck = _ducks[i];
                if (duck.Column != e.Column) continue;

                for (int r = duck.Row + 1; r < _grid.Rows; r++)
                {
                    if (_grid.GetBlock(r, duck.Column) != null) return;
                }
                _grid.SetBlock(duck.Row, duck.Column, null);
                _factory.RecycleBlock(duck);
                _ducks.RemoveAt(i);

                _eventBus.Fire(new DuckDeliveredEvent(duck.Column));
            }
        }
    }

}