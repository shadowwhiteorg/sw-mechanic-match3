using _Game.Enums;

namespace _Game.Systems.BlockSystem
{
    /// <summary>
    /// Data model for a single block: its type, grid coordinates, and view.
    /// </summary>
    public class BlockModel
    {
        public BlockType Type { get; private set; }
        public int Row { get; private set; }
        public int Column { get; private set; }
        public BlockView View { get; private set; }

        public BlockModel(BlockType type, int row, int column, BlockView view)
        {
            Type = type;
            Row = row;
            Column = column;
            View = view;
        }

        public void SetGridPosition(int row, int column)
        {
            Row = row;
            Column = column;
        }
    }
}