﻿using _Game.Enums;
using _Game.Interfaces;
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
        public bool TryGet(int row, int col, out BlockModel block)
        {
            if (row < 0 || row >= Rows || col < 0 || col >= Columns)
            {
                block = null;
                return false;
            }

            block = GetBlock(row, col);
            return block != null;
        }

        public void ClearAllColoredBlocks()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if(GetBlock(row,col).Type != BlockType.None) continue;
                    SetBlock(row, col, null);
                }
            }
            
        }
    }
}