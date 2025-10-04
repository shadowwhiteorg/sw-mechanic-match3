using _Game.Core.Events;
using _Game.Enums;
using _Game.Interfaces;
using _Game.Systems.BlockSystem;
using _Game.Systems.BlockSystem.Components;
using UnityEngine;

namespace _Game.Systems.BehaviorSystem
{
    [CreateAssetMenu(menuName = "Blast/Behaviors/Box")]
    public class BoxBehaviorAsset : BlockBehaviorAsset
    {
        [SerializeField] private int maxHealth = 1;
        
        private IHealthComponent _healthComponent;
        
        public override void OnPlaced(BlockModel block)
        {
            base.OnPlaced(block);
            _healthComponent = new HealthComponent(block, Events, maxHealth);
            
            // Subscribe to damage events
            Events.Subscribe<BlockDamagedEvent>(OnBlockDamaged);
            Events.Subscribe<BlockDestroyedEvent>(OnBlockDestroyed);
        }
        
        public override void OnCleared(BlockModel block)
        {
            // Unsubscribe from events when cleared
            Events.Unsubscribe<BlockDamagedEvent>(OnBlockDamaged);
            Events.Unsubscribe<BlockDestroyedEvent>(OnBlockDestroyed);
        }
        
        public override bool CanClear(BlockModel block)
        {
            // Box can only be cleared when it has no health
            return _healthComponent != null && !_healthComponent.IsAlive;
        }
        
        public override void OnMatched(BlockModel block)
        {
            // Box doesn't clear on match, only when destroyed by damage
        }
        
        public override void OnFell(BlockModel block)
        {
            // Box doesn't fall down
        }
        
        private void OnBlockDamaged(BlockDamagedEvent e)
        {
            // Check if this block was damaged
            if (e.Block == null) return;
            
            // Find the block in the grid and check if it's a box
            if (Grid.TryGet(e.Block.Row, e.Block.Column, out var block) && 
                block.Type == BlockType.Box)
            {
                // If the box is destroyed, clear it
                if (!_healthComponent.IsAlive)
                {
                    Events.Fire(new ClearBlockEvent(block));
                }
            }
        }
        
        private void OnBlockDestroyed(BlockDestroyedEvent e)
        {
            // Check if this block was destroyed
            if (e.Block == null) return;
            
            // Find the block in the grid and check if it's a box
            if (Grid.TryGet(e.Block.Row, e.Block.Column, out var block) && 
                block.Type == BlockType.Box)
            {
                // Clear the destroyed box
                Events.Fire(new ClearBlockEvent(block));
            }
        }
    }
}
