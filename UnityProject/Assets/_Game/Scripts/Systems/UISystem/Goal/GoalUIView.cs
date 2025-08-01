﻿using System.Collections.Generic;
using _Game.Enums;
using _Game.Systems.BlockSystem;
using UnityEngine;

namespace _Game.Systems.UISystem
{
    public class GoalUIView : BaseUIView
    {
        [Header("Layout")]
        [SerializeField] private Transform goalContainer;
        [SerializeField] private GoalItemView itemPrefab;

        [Header("Config")]
        [SerializeField] private BlockTypeConfig blockTypeConfig;

        private readonly Dictionary<BlockColor, GoalItemView> _colorViews = new();
        private readonly Dictionary<BlockType, GoalItemView>  _typeViews  = new();
        
        protected override void OnViewUpdated()
        {
            var model = (GoalUIModel)Model;

            // ensure we have a view for each goal state
            if (_colorViews.Count < model.ColorStates.Count ||
                _typeViews .Count < model.TypeStates .Count)
            {
                foreach (var c in model.ColorStates)
                    if (!_colorViews.ContainsKey(c.Color))
                        _colorViews[c.Color] = Instantiate(itemPrefab, goalContainer);

                foreach (var t in model.TypeStates)
                    if (!_typeViews.ContainsKey(t.Type))
                        _typeViews[t.Type] = Instantiate(itemPrefab, goalContainer);
            }

            RefreshAll();
        }

        private void RefreshAll()
        {
            foreach (var state in ((GoalUIModel)Model).ColorStates)
            {
                var entry = blockTypeConfig.Get(state.Color, BlockType.None);
                _colorViews[state.Color].Set(state.Remaining, entry.Sprite);
            }

            foreach (var state in ((GoalUIModel)Model).TypeStates)
            {
                var entry = blockTypeConfig.Get(BlockColor.None, state.Type);
                _typeViews[state.Type].Set(state.Remaining, entry.Sprite);
            }
        }

        public Vector3 GetGoalItemWorldPosition(BlockColor color)
        {
            if (_colorViews.TryGetValue(color, out var view))
                return view.transform.position;
            return Vector3.zero;
        }

        public Vector3 GetGoalItemWorldPosition(BlockType type)
        {
            if (_typeViews.TryGetValue(type, out var view))
                return view.transform.position;
            return Vector3.zero;
        }
    }
}
