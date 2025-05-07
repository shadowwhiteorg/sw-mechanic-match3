using System.Collections.Generic;
using System.Linq;
using _Game.Enums;
using _Game.Interfaces;

namespace _Game.Scripts.Systems.BehaviorSystem
{
    // e) A tiny registry you wire up via DI on game startup
    public class SpecialBehaviorRegistry
    {
        private readonly Dictionary<SpecialBlockType, ISpecialBlockBehavior> _lookup;

        public SpecialBehaviorRegistry(IEnumerable<ISpecialBlockBehavior> behaviors)
        {
            _lookup = behaviors.ToDictionary(b => b.HandledType);
        }

        public ISpecialBlockBehavior Get(SpecialBlockType t) 
            => _lookup.TryGetValue(t, out var b) ? b : null;
    }

}