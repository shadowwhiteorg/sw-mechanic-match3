using _Game.Core.Events;
using _Game.Interfaces;
using _Game.Systems.BlockSystem;
using UnityEngine;

namespace _Game.Systems.BlockSystem.Components
{
    public class HealthComponent : IHealthComponent
    {
        private int _currentHealth;
        private int _maxHealth;
        private BlockModel _block;
        private IEventBus _events;
        
        public int CurrentHealth => _currentHealth;
        public int MaxHealth => _maxHealth;
        public bool IsAlive => _currentHealth > 0;
        
        public HealthComponent(BlockModel block, IEventBus eventBus, int maxHealth = 1)
        {
            _block = block;
            _events = eventBus;
            _maxHealth = maxHealth;
            _currentHealth = maxHealth;
        }
        
        public void TakeDamage(int damage, DamageSource source)
        {
            if (!IsAlive) return;
            
            _currentHealth = Mathf.Max(0, _currentHealth - damage);
            
            _events.Fire(new BlockDamagedEvent(_block, damage, source));
            
            if (!IsAlive)
            {
                _events.Fire(new BlockDestroyedEvent(_block, source));
            }
        }
        
        public void Heal(int amount)
        {
            _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);
        }
        
        public void SetMaxHealth(int maxHealth)
        {
            _maxHealth = maxHealth;
            _currentHealth = Mathf.Min(_currentHealth, _maxHealth);
        }
    }
}
