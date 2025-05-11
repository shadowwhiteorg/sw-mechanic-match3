using System.Linq;
using _Game.Core.Events;
using _Game.Enums;
using _Game.Interfaces;
using UnityEngine;

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

            // Update color goals
            int colorIndex = _level.ColorGoals.FindIndex(g => g.Color == b.Color && g.Count > 0);
            if (colorIndex >= 0)
            {
                var goal = _level.ColorGoals[colorIndex];
                goal.Count--;
                _level.ColorGoals[colorIndex] = goal;
                UpdateColorGoal(goal.Color, goal.Count);
                Debug.Log($"Goal {goal.Color} Updated: {goal.Count}");
            }

            // Update type goals
            int typeIndex = _level.TypeGoals.FindIndex(g => g.Type == b.Type && g.Count > 0);
            if (typeIndex >= 0)
            {
                var goal = _level.TypeGoals[typeIndex];
                goal.Count--;
                _level.TypeGoals[typeIndex] = goal;
                UpdateTypeGoal(goal.Type, goal.Count);
                Debug.Log($"Goal {goal.Type} Updated: {goal.Count}");
            }

            // Check for completion
            if (IsLevelComplete())
            {
                _events.Fire(new LevelCompleteEvent());
                Debug.Log("Level complete!");
            }
        }


        private void UpdateColorGoal(BlockColor color, int remaining)
        {
            _events.Fire(new GoalUpdatedEvent(
                GoalUpdatedEvent.GoalCategory.Color, (int)color, remaining));
            Debug.Log($"Color goal {color} updated to {remaining}");
        }

        private void UpdateTypeGoal(BlockType type, int remaining)
        {
            _events.Fire(new GoalUpdatedEvent(
                GoalUpdatedEvent.GoalCategory.Type, (int)type, remaining));
            Debug.Log($"Type goal {type} updated to {remaining}");
        }

        private bool IsLevelComplete()
        {
            return _level.ColorGoals.All(g => g.Count <= 0)
                   && _level.TypeGoals.All(g => g.Count <= 0);
        }
    }
}
