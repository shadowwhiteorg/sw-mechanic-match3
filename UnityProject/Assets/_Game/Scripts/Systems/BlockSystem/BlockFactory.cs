﻿using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core.Events;
using UnityEngine;
using _Game.Enums;
using _Game.Interfaces;
using _Game.Systems.BehaviorSystem;
using _Game.Systems.BlockSystem;
using _Game.Utils;

namespace _Game.Systems.GridSystem
{
    public class BlockFactory : IBlockFactory
    {
        private readonly IGridHandler _grid;
        private readonly GridWorldHelper _helper;
        private readonly IEventBus _events;
        private readonly ObjectPool<BlockView> _viewPool;
        private readonly BlockTypeConfig _config;
        private readonly Transform _parent;
        private readonly BehaviorRegistry _registry;
        private readonly System.Random _rng = new();
        private bool _canSpawn = true; 

        public BlockFactory(
            BlockTypeConfig config,
            BlockView viewPrefab,
            Transform parent,
            GridWorldHelper helper,
            IGridHandler grid,
            IEventBus events,
            BehaviorRegistry registry,
            int initialPoolSize = 100)
        {
            _config = config;
            _parent = parent;
            _helper = helper;
            _grid = grid;
            _events = events;
            _registry = registry;
            _viewPool = new ObjectPool<BlockView>(viewPrefab, initialPoolSize, parent);
            _events.Subscribe<LevelCompleteEvent>(e => Activate(false));
            _events.Subscribe<GameOverEvent>(e => Activate(false));
            _events.Subscribe<LevelInitializedEvent>(e => Activate(true));
        }

        public BlockModel CreateBlock(BlockColor color, BlockType type,BlockDirection direction, int row, int col)
        {
            if(!_canSpawn) return null;
            var entry = _config.Get(color, type);
            var view = _viewPool.Get();
            view.transform.SetParent(_parent, false);
            view.transform.position = _helper.GetWorldPosition(row, col);
            view.transform.rotation = Quaternion.identity;
            if(direction == BlockDirection.Vertical)
                view.transform.Rotate(0, 0, 90f);
            view.SetSprite(entry.Sprite);

            var behaviors = new List<IBlockBehavior>();
            foreach (var asset in _registry.GetBehaviorsFor(type))
            {
                asset.Initialize(_grid, this, _helper, _events);
                behaviors.Add(asset);
            }

            var model = new BlockModel(color, type, row, col, view, behaviors, direction);
            _grid.SetBlock(row, col, model);
            return model;
        }

        public BlockModel CreateRandomBlock(int row, int col)
        {
            if(!_canSpawn) return null;
            var color = Enum.GetValues(typeof(BlockColor)).Cast<BlockColor>().OrderBy(_ => _rng.Next()).First();
            var type = BlockType.None;
            var direction = BlockDirection.None;
            if (color == BlockColor.None) color = BlockColor.Red;
            return CreateBlock(color, type, direction,row, col);
        }

        public void RecycleBlock(BlockModel model)
        {
            _grid.SetBlock(model.Row, model.Column, null);
            model.View.gameObject.SetActive(false);
            _viewPool.Return(model.View);
        }

        private void Activate(bool active)
        {
            _canSpawn = active;
        }
    }
}
