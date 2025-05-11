using System.Collections.Generic;
using _Game.Enums;
using _Game.Systems.BlockSystem;
using UnityEditor;
using UnityEngine;

namespace _Game.Systems.UISystem
{
    public class GoalUIView : BaseUIView
    {
        [Header("Layout")]
        [SerializeField] private Transform _colorGoalContainer;
        [SerializeField] private Transform _typeGoalContainer;
        [SerializeField] private GoalItemView _itemPrefab;

        [Header("Config")]
        [SerializeField] private BlockTypeConfig _blockTypeConfig;

        private readonly Dictionary<BlockColor, GoalItemView> _colorViews = new();
        private readonly Dictionary<BlockType, GoalItemView> _typeViews = new();

        protected override void OnBind()
        {
            foreach (var colorState in ((GoalUIModel)Model).ColorStates)
            {
                var view = Instantiate(_itemPrefab, _colorGoalContainer);
                _colorViews[colorState.Color] = view;
            }

            foreach (var typeState in ((GoalUIModel)Model).TypeStates)
            {
                var view = Instantiate(_itemPrefab, _typeGoalContainer);
                _typeViews[typeState.Type] = view;
            }

            RefreshAll();
        }

        protected override void OnViewUpdated()
        {
            RefreshAll();
        }

        private void RefreshAll()
        {
            foreach (var colorState in ((GoalUIModel)Model).ColorStates)
            {
                var entry = _blockTypeConfig.Get(colorState.Color, BlockType.None);
                _colorViews[colorState.Color].Set(colorState.Remaining, entry.Sprite);
            }

            foreach (var typeState in ((GoalUIModel)Model).TypeStates)
            {
                var entry = _blockTypeConfig.Get(BlockColor.None, typeState.Type);
                _typeViews[typeState.Type].Set(typeState.Remaining, entry.Sprite);
            }
        }
    }
}