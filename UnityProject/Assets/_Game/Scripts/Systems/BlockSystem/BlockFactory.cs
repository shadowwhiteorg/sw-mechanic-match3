using UnityEngine;
using _Game.Enums;

namespace _Game.Systems.BlockSystem
{
    /// <summary>
    /// Responsible for instantiating blocks of the correct type/color in the scene.
    /// </summary>
    public class BlockFactory
    {
        private readonly BlockTypeConfig _typeConfig;
        private readonly GameObject      _blockPrefab;
        private readonly Transform       _parent;

        public BlockFactory(BlockTypeConfig typeConfig, GameObject blockPrefab, Transform parent)
        {
            _typeConfig  = typeConfig;
            _blockPrefab = blockPrefab;
            _parent      = parent;
        }

        /// <summary>Creates a block of a specific type at the given grid coords.</summary>
        public BlockModel CreateBlock(BlockType type, int row, int column, Vector3 worldPosition)
        {
            var go   = Object.Instantiate(_blockPrefab, worldPosition, Quaternion.identity, _parent);
            var view = go.GetComponent<BlockView>();
            view.SetSprite(_typeConfig.GetSprite(type));
            return new BlockModel(type, row, column, view);
        }

        /// <summary>Creates a random‐type block.</summary>
        public BlockModel CreateRandomBlock(int row, int column, Vector3 worldPosition)
        {
            var values     = System.Enum.GetValues(typeof(BlockType));
            var randomType = (BlockType)values.GetValue(Random.Range(0, values.Length));
            return CreateBlock(randomType, row, column, worldPosition);
        }
    }
}