using _Game.Enums;
using UnityEngine;
using _Game.Interfaces;
using _Game.Systems.BlockSystem;
using _Game.Systems.GridSystem;
using _Game.Utils;

namespace _Game.Systems.BehaviorSystem
{
    public abstract class BlockBehaviorAsset : ScriptableObject, IBlockBehavior
    {
        [SerializeField] private BlockType type;

        protected IGridHandler    Grid;
        protected BlockFactory    Factory;
        protected GridWorldHelper Helper;
        protected IEventBus       Events;
        
        public virtual BlockType Type => type;

        public virtual void Initialize(
            IGridHandler grid,
            BlockFactory factory,
            GridWorldHelper helper,
            IEventBus eventBus
        )
        {
            Grid     = grid;
            Factory  = factory;
            Helper   = helper;
            Events = eventBus;
        }

        public virtual void OnPlaced(BlockModel b) {}
        public virtual void OnMatched(BlockModel b) {}
        public virtual void OnCleared(BlockModel b) {}
        public virtual void OnFell(BlockModel b) {}
        public virtual void OnTurnStart(BlockModel b) {}

    }
}