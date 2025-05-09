// --- BlockBehaviorAsset.cs ---

using _Game.Enums;
using UnityEngine;
using _Game.Interfaces;
using _Game.Systems.GridSystem;
using _Game.Systems.BlockSystem;
using _Game.Utils;

namespace _Game.Systems.BehaviorSystem
{
    public abstract class BlockBehaviorAsset : ScriptableObject, IBlockBehavior
    {
        [SerializeField] private BlockType type;
        
        protected IGridHandler Grid;
        protected IBlockFactory Factory;
        protected GridWorldHelper Helper;
        protected IEventBus Events;
        public virtual BlockType Type => type;
        public virtual void Initialize(
            IGridHandler grid,
            IBlockFactory factory,
            GridWorldHelper helper,
            IEventBus eventBus
        ) {
            Grid    = grid;
            Factory = factory;
            Helper  = helper;
            Events  = eventBus;
        }

        public virtual void OnPlaced(BlockModel block)       {}
        public virtual void OnActivated(BlockModel block)    {} // default no‐op
        public virtual void OnMatched(BlockModel block)      {}
        public virtual void OnCleared(BlockModel block)      {}
        public virtual void OnFell(BlockModel block)         {}
        public virtual void OnTurnStart(BlockModel block)    {}
    }
}