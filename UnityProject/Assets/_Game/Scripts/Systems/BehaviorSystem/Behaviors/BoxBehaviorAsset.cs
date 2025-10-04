using _Game.Core.Events;
using _Game.Enums;
using _Game.Systems.BlockSystem;
using UnityEngine;

namespace _Game.Systems.BehaviorSystem
{
    [CreateAssetMenu(menuName = "Blast/Behaviors/Box")]
    public class BoxBehaviorAsset : BlockBehaviorAsset
    {
        [SerializeField] private int maxHealth = 1;
        
        private int _currentHealth;
        private bool _isDestroyed = false;
        private BlockModel _block;
        
        public override void OnPlaced(BlockModel block)
        {
            base.OnPlaced(block);
            _block = block;
            _currentHealth = maxHealth;
            _isDestroyed = false;
            
            // Subscribe to damage events
            Events.Subscribe<BlockDamagedEvent>(OnBlockDamaged);
        }
        
        public override void OnCleared(BlockModel block)
        {
            // Unsubscribe from events when cleared
            Events.Unsubscribe<BlockDamagedEvent>(OnBlockDamaged);
        }
        
        public override bool CanClear(BlockModel block)
        {
            // Box can only be cleared when it's destroyed
            return _isDestroyed;
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
            if (e.Block != null && e.Block == _block)
            {
                if (_isDestroyed) return;
                
                _currentHealth -= e.Damage;
                
                if (_currentHealth <= 0)
                {
                    _isDestroyed = true;
                    Events.Fire(new BlockDestroyedEvent(_block, e.Source));
                    Events.Fire(new ClearBlockEvent(_block));
                }
            }
        }
    }
}
