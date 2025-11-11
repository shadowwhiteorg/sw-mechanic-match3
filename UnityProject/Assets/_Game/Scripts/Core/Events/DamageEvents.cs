using _Game.Interfaces;
using _Game.Systems.BlockSystem;

namespace _Game.Core.Events
{
    public struct BlockDamagedEvent : IGameEvent
    {
        public BlockModel Block { get; }
        public int Damage { get; }
        public DamageSource Source { get; }
        
        public BlockDamagedEvent(BlockModel block, int damage, DamageSource source)
        {
            Block = block;
            Damage = damage;
            Source = source;
        }
    }
    
    public struct BlockDestroyedEvent : IGameEvent
    {
        public BlockModel Block { get; }
        public DamageSource Source { get; }
        
        public BlockDestroyedEvent(BlockModel block, DamageSource source)
        {
            Block = block;
            Source = source;
        }
    }
    
    public enum DamageSource
    {
        Blast,
        Rocket,
        Bomb
    }
}
