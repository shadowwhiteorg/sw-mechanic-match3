using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Game.Core.Events;
using _Game.Enums;
using _Game.Interfaces;
using _Game.Systems.BehaviorSystem;
using _Game.systems.BlockSystem;
using _Game.Systems.BlockSystem;
using _Game.Utils;
using UnityEngine;

namespace _Game.Systems.MatchSystem
{
    public class ClearSystem
    {
        private readonly IGridHandler _grid;
        private readonly IBlockFactory _factory;
        private readonly IEventBus _events;
        private readonly SpecialBlockSpawnConfig _spawnConfig;

        private readonly List<(int row, int col)> _pending = new();
        private int _activeBlockCount = 0;
        private bool _batching = false;
        private bool _flushScheduled = false;
        private int _currentBlastGroupId = 0;

        public ClearSystem(
            IGridHandler grid,
            IBlockFactory factory,
            IEventBus events,
            SpecialBlockSpawnConfig spawnConfig)
        {
            _grid        = grid;
            _factory     = factory;
            _events      = events;
            _spawnConfig = spawnConfig;

            _events.Subscribe<MatchFoundEvent>(   OnMatchFound);
            _events.Subscribe<BlockSelectedEvent>(OnBlockSelected);
            _events.Subscribe<ClearBlockEvent>(   OnClearBlock);
            _events.Subscribe<BlockActivatedEvent>(@event=>_activeBlockCount++);
            _events.Subscribe<BlockDeactivatedEvent>(@event=>_activeBlockCount--);
        }

        // MatchFound starts a coroutine that clears the match and spawns the special
        private void OnMatchFound(MatchFoundEvent e)
        {
            _currentBlastGroupId++; // Increment blast group ID for each match
            CoroutineRunner.Instance.StartCoroutine(ClearMatchAndSpawnSpecial(e));
        }

        private IEnumerator ClearMatchAndSpawnSpecial(MatchFoundEvent e)
        {
            _batching = true;

            foreach (var blk in e.Blocks)
                _events.Fire(new ClearBlockEvent(blk));

            yield return null; // wait a frame to finish all clears

            var specialType = _spawnConfig.GetTypeForMatch(e.Blocks.Count);
            if (specialType != BlockType.None)
            {
                var color = BlockColor.None;
                var direction = specialType == BlockType.Rocket ? (BlockDirection)Random.Range(1, 3) : BlockDirection.None;
                
                var special = _factory.CreateBlock(color, specialType,direction, e.TouchOrigin.x, e.TouchOrigin.y);
                special.Settle(true); // protect from falling
            }

            Flush();
            _batching = false;
        }

        // Special blocks like bombs/rockets/ducks trigger here
        private void OnBlockSelected(BlockSelectedEvent e)
        {
            if (!_grid.TryGet(e.Row, e.Col, out var blk) || blk == null)
                return;
            blk.Activated();
        }

        // Called for every block cleared by match or special
        private void OnClearBlock(ClearBlockEvent e)
        {
            var blk = e.Block;
            Debug.Log($"ClearSystem: OnClearBlock called for block at ({blk.Row}, {blk.Column}) of type {blk.Type}");
            
            if(!blk.CanClear())
            {
                Debug.Log($"ClearSystem: Block at ({blk.Row}, {blk.Column}) cannot be cleared (CanClear returned false)");
                return;
            }
            
            if (!blk.IsSettled)
            {
                Debug.Log($"ClearSystem: Block at ({blk.Row}, {blk.Column}) is not settled");
                return;
            }
            
            if (!_grid.TryGet(blk.Row, blk.Column, out var live) || live != blk)
            {
                Debug.Log($"ClearSystem: Block at ({blk.Row}, {blk.Column}) not found in grid or not the same instance");
                return;
            }
            
            Debug.Log($"ClearSystem: Processing clear for block at ({blk.Row}, {blk.Column})");
            
            // Only damage adjacent boxes for regular blasts, not for box or vase destruction
            if (blk.Type != BlockType.Box && blk.Type != BlockType.Vase)
            {
                DamageAdjacentBoxes(blk.Row, blk.Column);
            }
            
            // Remove from grid and buffer
            _grid.SetBlock(blk.Row, blk.Column, null);
            _factory.RecycleBlock(blk);
            _pending.Add((blk.Row, blk.Column));
            blk.Cleared();
            
            Debug.Log($"ClearSystem: Block at ({blk.Row}, {blk.Column}) cleared successfully");
            
            // If not inside match-batch, flush at end of frame (once)
            if (!_batching && !_flushScheduled)
            {
                _flushScheduled = true;
                Flush();
            }
        }
        
        private void DamageAdjacentBoxes(int centerRow, int centerCol)
        {
            // Check only 4 cardinal directions (North, South, East, West)
            int[,] directions = { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 } }; // N, S, W, E
            
            for (int i = 0; i < 4; i++)
            {
                int dr = directions[i, 0];
                int dc = directions[i, 1];
                
                int targetRow = centerRow + dr;
                int targetCol = centerCol + dc;
                
                if (_grid.TryGet(targetRow, targetCol, out var block) && block != null)
                {
                    if (block.Type == BlockType.Box)
                    {
                        Debug.Log($"Found box at ({targetRow}, {targetCol}) - block hash: {block.GetHashCode()}");
                        var boxBehavior = block.GetBehavior<BoxBehaviorAsset>();
                        if (boxBehavior != null)
                        {
                            Debug.Log($"Calling TakeDamage with block at ({block.Row}, {block.Column})");
                            boxBehavior.TakeDamage(1, DamageSource.Blast, block);
                        }
                    }
                    else if (block.Type == BlockType.Stone)
                    {
                        Debug.Log($"Found stone at ({targetRow}, {targetCol}) - block hash: {block.GetHashCode()}");
                        var stoneBehavior = block.GetBehavior<StoneBehaviorAsset>();
                        if (stoneBehavior != null)
                        {
                            Debug.Log($"Calling TakeDamage with stone at ({block.Row}, {block.Column}) - will be rejected");
                            stoneBehavior.TakeDamage(1, DamageSource.Blast, block);
                        }
                    }
                    else if (block.Type == BlockType.Vase)
                    {
                        Debug.Log($"Found vase at ({targetRow}, {targetCol}) - block hash: {block.GetHashCode()}");
                        var vaseBehavior = block.GetBehavior<VaseBehaviorAsset>();
                        if (vaseBehavior != null)
                        {
                            Debug.Log($"Calling TakeDamage with vase at ({block.Row}, {block.Column}) - blast group {_currentBlastGroupId}");
                            vaseBehavior.TakeDamage(1, DamageSource.Blast, block, _currentBlastGroupId);
                        }
                        else
                        {
                            Debug.LogWarning($"Vase behavior not found for vase at ({block.Row}, {block.Column})");
                        }
                    }
                }
            }
        }

        private IEnumerator FlushCoroutine()
        {
            yield return new WaitUntil(() => _activeBlockCount == 0);
            _events.Fire(new BlocksClearedEvent(_pending.ToList()));
            if (_pending.Count != 0)
                _pending.Clear();
            _flushScheduled = false;
            
        }

        private void Flush()
        {
            CoroutineRunner.Instance.StartCoroutine(FlushCoroutine());
        }
        
    }
}
