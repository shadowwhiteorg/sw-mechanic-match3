using _Game.Interfaces;
using _Game.Systems.BlockSystem;
using _Game.Systems.GridSystem;
using _Game.Utils;

namespace _Game.Interfaces
{
    public interface IBlockBehavior
    {
        void Initialize(
            IGridHandler grid,
            BlockFactory factory,
            GridWorldHelper helper,
            IEventBus eventBus
        );

        void OnPlaced(BlockModel block);
        void OnMatched(BlockModel block);
        void OnCleared(BlockModel block);
        void OnFell(BlockModel block);
        void OnTurnStart(BlockModel block);
    }
}