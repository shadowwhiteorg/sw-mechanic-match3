using _Game.Enums;
using _Game.Systems.BlockSystem;
using _Game.Systems.GridSystem;

namespace _Game.Interfaces
{
    public interface ISpecialBlockBehavior
    {
        SpecialBlockType HandledType { get; }
        void Activate(BlockModel block, GridHandler grid = null);
    }
}