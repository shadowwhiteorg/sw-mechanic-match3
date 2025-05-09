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
            IBlockFactory factory,
            GridWorldHelper helper,
            IEventBus eventBus
        );

        void OnPlaced(BlockModel block);
        void OnActivated(BlockModel block); // NEW: tap or programmatic activation
        void OnMatched(BlockModel block);// match‐group activation
        void OnCleared(BlockModel block);
        void OnFell(BlockModel block);
        void OnTurnStart(BlockModel block);
        void OnTurnEnd();
        bool CanClear(BlockModel block);
    }
}