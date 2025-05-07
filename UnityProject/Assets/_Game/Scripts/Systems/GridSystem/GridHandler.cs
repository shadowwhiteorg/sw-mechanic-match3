using _Game.Scripts.Interfaces;
using _Game.Systems.BlockSystem;

namespace _Game.Systems.GridSystem
{
    public class GridHandler : IGridHandler
    {
        private readonly BlockModel[,] _blocks;

        public int Rows    { get; }
        public int Columns { get; }

        public GridHandler(int rows, int columns)
        {
            Rows    = rows;
            Columns = columns;
            _blocks = new BlockModel[rows, columns];
        }

        public void SetBlock(int row, int column, BlockModel block)
        {
            _blocks[row, column] = block;
            if(block == null) return;
            block.SetGridPosition(row, column);
        }

        public BlockModel GetBlock(int row, int column)
        {
            return IsInside(row, column) ? _blocks[row, column] : null;
        }

        public bool IsInside(int row, int column)
        {
            return row >= 0 && row < Rows && column >= 0 && column < Columns;
        }
    }
}