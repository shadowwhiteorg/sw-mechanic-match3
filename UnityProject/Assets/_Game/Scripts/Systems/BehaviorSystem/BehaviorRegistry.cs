using System.Collections.Generic;
using _Game.Enums;
using _Game.Interfaces;

namespace _Game.Systems.BehaviorSystem
{
    public class BehaviorRegistry
    {
        private readonly Dictionary<BlockType, List<IBlockBehavior>> _map = new();

        public BehaviorRegistry(IEnumerable<BlockBehaviorAsset> assets)
        {
            foreach (var asset in assets)
            {
                if (!_map.ContainsKey(asset.Type))
                    _map[asset.Type] = new List<IBlockBehavior>();

                _map[asset.Type].Add(asset);
            }
        }

        public List<IBlockBehavior> GetBehaviorsFor(BlockType type)
        {
            return _map.TryGetValue(type, out var list) ? list : new List<IBlockBehavior>();
        }
    }

}