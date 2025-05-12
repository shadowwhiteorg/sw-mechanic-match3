using _Game.Interfaces;
using _Game.Utils;

namespace _Game.Systems.GameLoop
{
    public class LevelInitializer
    {
        private readonly IGridHandler    _grid;
        private readonly IBlockFactory   _factory;
        private readonly GridWorldHelper _helper;
        private readonly LevelData       _level;

        public LevelInitializer(
            IGridHandler grid,
            IBlockFactory factory,
            GridWorldHelper helper,
            LevelData level)
        {
            _grid    = grid;
            _factory = factory;
            _helper  = helper;
            _level   = level;
        }

        public void Initialize()
        {
            int cols = _level.Columns;
            for (int idx = 0; idx < _level.InitialBlocks.Count; idx++)
            {
                int r = idx / cols;
                int c = idx % cols;
                var def = _level.InitialBlocks[idx];

                var block = _factory.CreateBlock(def.Color, def.Type,def.Direction, r, c);
                block.View.transform.position = _helper.GetWorldPosition(r, c);
                block.Settle(true);
            }
        }
    }
}