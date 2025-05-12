using System.Linq;
using UnityEngine;
using _Game.Core.Events;
using _Game.Enums;
using _Game.Interfaces;
using _Game.Systems.BlockSystem;
using _Game.Utils;

namespace _Game.Systems.UISystem
{
    /// <summary>
    /// Spawns a flying sprite whenever a goal block is cleared,
    /// tweening it into the matching GoalItemView slot.
    /// Deterministically checks the model at clear time.
    /// </summary>
    public class GoalFlyerService
    {
        private readonly IEventBus        _events;
        private readonly BlockTypeConfig  _config;
        private readonly GridWorldHelper  _helper;
        private readonly GoalUIView       _goalView;
        private readonly GoalUIModel      _goalModel;
        private readonly GameObjectPool   _pool;

        public GoalFlyerService(
            IEventBus       events,
            BlockTypeConfig config,
            GridWorldHelper helper,
            GoalUIView      goalView,
            GoalUIModel     goalModel,
            GameObjectPool  pool)
        {
            _events    = events;
            _config    = config;
            _helper    = helper;
            _goalView  = goalView;
            _goalModel = goalModel;
            _pool      = pool;

            _events.Subscribe<ClearBlockEvent>(OnBlockCleared);
        }

        private void OnBlockCleared(ClearBlockEvent e)
        {
            var block = e.Block;

            // If this block still counts toward a color goal, spawn a flyer
            if (_goalModel.ColorStates.Any(s => s.Color == block.Color && s.Remaining > 0))
            {
                SpawnFlyer(
                    _helper.GetWorldPosition(block.Row, block.Column),
                    _config.Get(block.Color, BlockType.None).Sprite,
                    _goalView.GetGoalItemWorldPosition(block.Color)
                );
            }

            // If it counts toward a type goal, spawn a flyer
            if (_goalModel.TypeStates.Any(s => s.Type == block.Type && s.Remaining > 0))
            {
                SpawnFlyer(
                    _helper.GetWorldPosition(block.Row, block.Column),
                    _config.Get(BlockColor.None, block.Type).Sprite,
                    _goalView.GetGoalItemWorldPosition(block.Type)
                );
            }
        }

        private void SpawnFlyer(Vector3 start, Sprite sprite, Vector3 target)
        {
            var go = _pool.Get();
            var sr = go.GetComponent<SpriteRenderer>();
            sr.sprite = sprite;
            go.transform.position = start;

            Tween.Position(
                go.transform,
                target,
                duration: 0.5f,
                ease: Ease.OutQuad,
                onComplete: () => _pool.Return(go)
            );
        }
    }
}
