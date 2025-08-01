﻿using _Game.Enums;
using _Game.Systems.BlockSystem;

namespace _Game.Interfaces
{
    public interface IBlockFactory
    {
        BlockModel CreateBlock(BlockColor color, BlockType special, BlockDirection direction,int row, int col);
        BlockModel CreateRandomBlock(int row, int col);
        void RecycleBlock(BlockModel model);
    }
}