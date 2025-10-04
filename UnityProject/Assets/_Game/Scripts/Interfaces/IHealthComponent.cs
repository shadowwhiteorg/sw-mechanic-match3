using _Game.Core.Events;

namespace _Game.Interfaces
{
    public interface IHealthComponent
    {
        int CurrentHealth { get; }
        int MaxHealth { get; }
        bool IsAlive { get; }
        
        void TakeDamage(int damage, DamageSource source);
        void Heal(int amount);
        void SetMaxHealth(int maxHealth);
    }
}
