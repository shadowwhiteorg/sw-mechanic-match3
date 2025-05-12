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
        [SerializeField] private Transform _goalContainer;
        [SerializeField] private GoalItemView _itemPrefab;

        [Header("Config")]
        [SerializeField] private BlockTypeConfig _blockTypeConfig;

        private readonly Dictionary<BlockColor, GoalItemView> _colorViews = new();
        private readonly Dictionary<BlockType, GoalItemView> _typeViews = new();

        protected override void OnBind()
        {
            // foreach (var colorState in ((GoalUIModel)Model).ColorStates)
            // {
            //     var view = Instantiate(_itemPrefab, _goalContainer);
            //     _colorViews[colorState.Color] = view;
            //     Debug.Log(colorState.Color + ": " + view);
            // }
            //
            // foreach (var typeState in ((GoalUIModel)Model).TypeStates)
            // {
            //     var view = Instantiate(_itemPrefab, _goalContainer);
            //     _typeViews[typeState.Type] = view;
            //     Debug.Log(typeState.Type + ": " + view);
            // }
            //
            // RefreshAll();
        }

        protected override void OnViewUpdated()
        {
            var model = (GoalUIModel)Model;
            // instantiate only when states appear
            if (_colorViews.Count < model.ColorStates.Count ||
                _typeViews .Count < model.TypeStates .Count)
            {
                foreach (var c in model.ColorStates)
                    if (!_colorViews.ContainsKey(c.Color))
                        _colorViews[c.Color] = Instantiate(_itemPrefab, _goalContainer);

                foreach (var t in model.TypeStates)
                    if (!_typeViews.ContainsKey(t.Type))
                        _typeViews[t.Type] = Instantiate(_itemPrefab, _goalContainer);
            }

            // now safe to refresh
            RefreshAll();
            RefreshAll();
        }

        private void RefreshAll()
        {
            foreach (var state in ((GoalUIModel)Model).ColorStates)
            {
                var entry = _blockTypeConfig.Get(state.Color, BlockType.None);
                _colorViews[state.Color].Set(state.Remaining, entry.Sprite);
            }

            foreach (var state in ((GoalUIModel)Model).TypeStates)
            {
                var entry = _blockTypeConfig.Get(BlockColor.None, state.Type);
                _typeViews[state.Type].Set(state.Remaining, entry.Sprite);
            }
        }
    }
}