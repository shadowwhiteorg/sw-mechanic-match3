using UnityEngine;

namespace _Game.Systems.GridSystem
{
    public class GridInitializer
    {
        private readonly GridHandler   _grid;
        private readonly BlockFactory _factory;
        private readonly GridConfig   _config;

        public GridInitializer(GridHandler grid, BlockFactory factory, GridConfig config)
        {
            _grid    = grid;
            _factory = factory;
            _config  = config;
        }
        public void InitializeGrid()
        {
            for (int r = 0; r < _config.Rows; r++)
            {
                for (int c = 0; c < _config.Columns; c++)
                {
                    Vector3 pos = _config.GetWorldPosition(r, c);
                    var block   = _factory.CreateRandomBlock(r, c);
                    block.View.SetPosition(pos);
                    _grid.SetBlock(r, c, block);
                }
            }
        }
    }
}