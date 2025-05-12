using System.Collections.Generic;
using System.Linq;
using _Game.Enums;
using _Game.Systems.GameLoop;

namespace _Game.Systems.UISystem
{
    public class GoalUIModel : BaseUIModel
    {
        private readonly List<ColorState> _colorStates = new();
        private readonly List<TypeState> _typeStates = new();

        public IReadOnlyList<ColorState> ColorStates => _colorStates;
        public IReadOnlyList<TypeState> TypeStates => _typeStates;

        public void SetGoals(IEnumerable<LevelData.ColorGoal> colorGoals, IEnumerable<LevelData.TypeGoal> typeGoals)
        {
            _colorStates.Clear();
            
            _colorStates.AddRange(colorGoals.Select(g => new ColorState
            {
                Color = g.Color,
                Remaining = g.Count
            }));

            _typeStates.Clear();
            _typeStates.AddRange(typeGoals.Select(g => new TypeState
            {
                Type = g.Type,
                Remaining = g.Count
            }));

            NotifyUpdated();
        }

        public void UpdateColorGoal(BlockColor color, int remaining)
        {
            var state = _colorStates.FirstOrDefault(s => s.Color == color);
            if (state != null)
            {
                state.Remaining = remaining;
                NotifyUpdated();
            }
        }

        public void UpdateTypeGoal(BlockType type, int remaining)
        {
            var state = _typeStates.FirstOrDefault(s => s.Type == type);
            if (state != null)
            {
                state.Remaining = remaining;
                NotifyUpdated();
            }
        }
    }
    
    public class ColorState
    {
        public BlockColor Color;
        public int Remaining;
        public bool IsComplete => Remaining <= 0;
    }

    public class TypeState
    {
        public BlockType Type;
        public int Remaining;
        public bool IsComplete => Remaining <= 0;
    }
}