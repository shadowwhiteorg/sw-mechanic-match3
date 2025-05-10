using System.Linq;
using _Game.Core.Events;
using _Game.Enums;
using _Game.Interfaces;

namespace _Game.Systems.GameLoop
{
    public class GoalSystem
    {
        private readonly LevelData _level;
        private readonly IEventBus _events;

        public GoalSystem(LevelData level, IEventBus events)
        {
            _level = level;
            _events = events;

            _events.Subscribe<ClearBlockEvent>(OnBlockCleared);
        }

        private void OnBlockCleared(ClearBlockEvent e)
        {
            var b = e.Block;

            // handle the player goals
            var cg = _level.ColorGoals.FirstOrDefault(x => x.Color == b.Color && x.Count > 0);
            if (cg.Count > 0)
            {
                cg.Count--;
                UpdateColorGoal(cg.Color, cg.Count);
            }

            // handle type goals
            var tg = _level.TypeGoals.FirstOrDefault(x => x.Type == b.Type && x.Count > 0);
            if (tg.Count > 0)
            {
                tg.Count--;
                UpdateTypeGoal(tg.Type, tg.Count);
            }

            // check for level completion
            if (IsLevelComplete())
                _events.Fire(new LevelCompleteEvent());
        }

        private void UpdateColorGoal(BlockColor color, int remaining)
        {
            _events.Fire(new GoalUpdatedEvent(
                GoalUpdatedEvent.GoalCategory.Color, (int)color, remaining));
        }

        private void UpdateTypeGoal(BlockType type, int remaining)
        {
            _events.Fire(new GoalUpdatedEvent(
                GoalUpdatedEvent.GoalCategory.Type, (int)type, remaining));
        }

        private bool IsLevelComplete()
        {
            return _level.ColorGoals.All(g => g.Count <= 0)
                   && _level.TypeGoals.All(g => g.Count <= 0);
        }
    }
}
