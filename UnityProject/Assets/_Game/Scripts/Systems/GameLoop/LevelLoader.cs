using System;
using System.Linq;
using _Game.Core.Events;
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
        private readonly IEventBus      _eventBus;

        public LevelLoader(IDIContainer container, IEventBus eventBus)
        {
            _grid    = container.Resolve<IGridHandler>();
            _factory = container.Resolve<IBlockFactory>();
            _helper  = container.Resolve<GridWorldHelper>();
            _eventBus = eventBus;
            _eventBus.Subscribe<LevelCompleteEvent>(e=>ClearLevel());
            _eventBus.Subscribe<GameOverEvent>(e=>ClearLevel());
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
                var color = def.Color;
                var type  = def.Type;
                if(type == BlockType.None && color == BlockColor.None)
                {
                    type = BlockType.None;
                    color = RandomColor();
                }

                var blk = _factory.CreateBlock(color, type, r, c);
                blk.View.transform.position = _helper.GetWorldPosition(r, c);
                blk.Settle(true);
            }
        }
        
        private void ClearLevel()
        {
            for (int r = 0; r < _grid.Rows; r++)
            for (int c = 0; c < _grid.Columns; c++)
                if (_grid.TryGet(r, c, out var b))
                {
                    _factory.RecycleBlock(b);
                    _grid.SetBlock(r, c, null);
                }
        }

        private BlockColor RandomColor()
        {
            var values = Enum.GetValues(typeof(BlockColor)).Cast<BlockColor>().Where(x => x != BlockColor.None).ToArray();
            return values[UnityEngine.Random.Range(0, values.Length)];
        }
    }
    
}