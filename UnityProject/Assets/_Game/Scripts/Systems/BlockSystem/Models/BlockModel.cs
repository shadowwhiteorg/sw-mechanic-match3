﻿using System.Collections.Generic;
using _Game.Enums;
using _Game.Interfaces;

namespace _Game.Systems.BlockSystem
{
    public class BlockModel
    {
        public BlockColor Color { get; }
        public BlockType Type { get; }
        public BlockDirection Direction { get; }
        public int Row { get; private set; }
        public int Column { get; private set; }
        public BlockView View { get; }

        private readonly List<IBlockBehavior> _behaviors;
        private bool _isClearing = false;
        private bool _isSettled = false;
        private bool _canClear = true;
        public bool CanClear()
        {
            foreach (var bhv in _behaviors)
            {
                if (!bhv.CanClear(this)) return false;
            }
            return true;
        }
        public bool IsSettled => _isSettled;
        

        public BlockModel(BlockColor color, BlockType type, int row, int col, BlockView view, IEnumerable<IBlockBehavior> behaviors, BlockDirection direction)
        {
            Color = color;
            Type = type;
            Direction = direction;
            Row = row;
            Column = col;
            View = view;
            _behaviors = new List<IBlockBehavior>(behaviors);

            foreach (var b in _behaviors)
                b.OnPlaced(this);
        }

        public void SetGridPosition(int newRow, int newCol)
        {
            Row = newRow;
            Column = newCol;
        }

        public void Matched()
        {
            foreach (var b in _behaviors) b.OnMatched(this);
        }

        public void Cleared()
        {
            if (_isClearing) return;
            _isClearing = true;
            foreach (var b in _behaviors) b.OnCleared(this);
        }
        
        public void Activated()
        {
            foreach (var b in _behaviors) b.OnActivated(this);
        }

        public void Fell()
        {
            foreach (var b in _behaviors) b.OnFell(this);
        }

        public void TurnStart()
        {
            foreach (var b in _behaviors) b.OnTurnStart(this);
        }
        public void TurnEnd()
        {
            foreach (var b in _behaviors) b.OnTurnEnd();
        }
        
        public void Settle( bool isSettled)
        {
            _isSettled = isSettled;
            
        }
    }
}
