// Systems/GridSystem/BlockFactory.cs
using UnityEngine;
using _Game.Data;
using _Game.Enums;
using _Game.Systems.BlockSystem;
using _Game.Utils;

namespace _Game.Systems.GridSystem
{
    public class BlockFactory
    {
        private readonly BlockTypeConfig _typeConfig;
        private readonly ObjectPool<BlockView> _viewPool;
        private readonly Transform _parent;

        public BlockFactory(
            BlockTypeConfig typeConfig,
            BlockView viewPrefab,
            Transform parent,
            int initialSize = 100)
        {
            _typeConfig = typeConfig;
            _parent      = parent;
            _viewPool    = new ObjectPool<BlockView>(viewPrefab, initialSize, parent);
        }

        public BlockModel CreateBlock(BlockType type, int row, int col, Vector3 worldPos)
        {
            var view = _viewPool.Get();
            view.transform.SetParent(_parent, false);
            view.SetSprite(_typeConfig.GetSprite(type));
            view.SetPosition(worldPos);
            view.gameObject.SetActive(true);
            return new BlockModel(type, row, col, view);
        }

        public void RecycleBlock(BlockModel block)
        {
            block.View.gameObject.SetActive(false);
            _viewPool.Return(block.View);
        }
        
        public BlockModel CreateRandomBlock(int row, int column, Vector3 worldPosition)
        {
            var values     = System.Enum.GetValues(typeof(BlockType));
            var randomType = (BlockType)values.GetValue(Random.Range(0, values.Length));
            return CreateBlock(randomType, row, column, worldPosition);
        }
    }
}