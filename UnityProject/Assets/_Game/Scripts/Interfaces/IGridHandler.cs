﻿using _Game.Systems.BlockSystem;

namespace _Game.Interfaces
{
    public interface IGridHandler
    {
        public int Rows    { get; }
        public int Columns { get; }
        void SetBlock(int row, int column, BlockModel block);
        BlockModel GetBlock(int row, int column);
        bool IsInside(int row, int column);
        public bool TryGet(int row, int col, out BlockModel block);
        void ClearAllColoredBlocks();

    }
}