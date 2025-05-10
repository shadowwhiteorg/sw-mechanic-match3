using System;
using System.Linq;
using _Game.Enums;
using _Game.Interfaces;
using _Game.Utils;

namespace _Game.Systems.GameLoop
{
    public class LevelLoader
    {
        private readonly IGridHandler    _grid;
        private readonly IBlockFactory   _factory;
        private readonly GridWorldHelper _helper;

        public LevelLoader(IDIContainer container)
        {
            _grid    = container.Resolve<IGridHandler>();
            _factory = container.Resolve<IBlockFactory>();
            _helper  = container.Resolve<GridWorldHelper>();
        }

        public void LoadLevel(LevelData level)
        {
            int cols = level.Columns;

            // clear existing
            for (int r = 0; r < level.Rows; r++)
            for (int c = 0; c < level.Columns; c++)
                if (_grid.TryGet(r, c, out var b))
                {
                    _factory.RecycleBlock(b);
                    _grid.SetBlock(r, c, null);
                }

            for (int i = 0; i < level.InitialBlocks.Count; i++)
            {
                int r = i / cols;
                int c = i % cols;

                var def = level.InitialBlocks[i];
                var color = def.Color == BlockColor.None ? RandomColor() : def.Color;
                var type  = def.Type;

                var blk = _factory.CreateBlock(color, type, r, c);
                blk.View.transform.position = _helper.GetWorldPosition(r, c);
                blk.Settle(true);
            }
        }

        private BlockColor RandomColor()
        {
            var values = Enum.GetValues(typeof(BlockColor)).Cast<BlockColor>().Where(x => x != BlockColor.None).ToArray();
            return values[UnityEngine.Random.Range(0, values.Length)];
        }
    }
    
}