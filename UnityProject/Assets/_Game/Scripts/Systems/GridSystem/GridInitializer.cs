using _Game.Core.Data;
using UnityEngine;

namespace _Game.Systems.GridSystem
{
    public class GridInitializer
    {
        private readonly GridSystem   _grid;
        private readonly BlockFactory _factory;
        private readonly GridConfig   _config;

        public GridInitializer(GridSystem grid, BlockFactory factory, GridConfig config)
        {
            _grid    = grid;
            _factory = factory;
            _config  = config;
        }

        /// <summary>
        /// Fills every cell with a randomly‐typed block.
        /// </summary>
        public void InitializeGrid()
        {
            for (int r = 0; r < _config.Rows; r++)
            {
                for (int c = 0; c < _config.Columns; c++)
                {
                    Vector3 pos = _config.GetWorldPosition(r, c);
                    var block   = _factory.CreateRandomBlock(r, c, pos);
                    block.View.SetPosition(pos);
                    _grid.SetBlock(r, c, block);
                }
            }
        }
    }
}