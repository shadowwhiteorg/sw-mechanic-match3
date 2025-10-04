using System.Collections.Generic;
using _Game.Core.Events;
using _Game.Enums;
using _Game.Interfaces;
using _Game.Systems.BlockSystem;
using UnityEngine;

namespace _Game.Systems.BlastSystem
{
    public class DamageSystem : IGameSystem
    {
        private readonly IGridHandler _grid;
        private readonly IEventBus _events;
        
        public DamageSystem(IGridHandler grid, IEventBus eventBus)
        {
            _grid = grid;
            _events = eventBus;
            
            // Subscribe to events that can cause damage
            _events.Subscribe<ClearBlockEvent>(OnBlockCleared);
            _events.Subscribe<BlockActivatedEvent>(OnBlockActivated);
        }
        
        private void OnBlockCleared(ClearBlockEvent e)
        {
            // When a block is cleared, check for adjacent boxes that should take damage
            CheckAdjacentDamage(e.Block.Row, e.Block.Column, DamageSource.Blast);
        }
        
        private void OnBlockActivated(BlockActivatedEvent e)
        {
            // This is called when special blocks like rockets are activated
            // The actual damage will be handled by the rocket behavior itself
        }
        
        public void OnRocketTraversal(int row, int col)
        {
            // Called by rocket behavior when it traverses a cell
            CheckCellDamage(row, col, DamageSource.Rocket);
        }
        
        public void OnBombExplosion(int centerRow, int centerCol, int radius)
        {
            // Called by bomb behavior when it explodes
            for (int dr = -radius; dr <= radius; dr++)
            {
                for (int dc = -radius; dc <= radius; dc++)
                {
                    if (Mathf.Max(Mathf.Abs(dr), Mathf.Abs(dc)) > radius)
                        continue;
                    
                    int targetRow = centerRow + dr;
                    int targetCol = centerCol + dc;
                    CheckCellDamage(targetRow, targetCol, DamageSource.Bomb);
                }
            }
        }
        
        private void CheckAdjacentDamage(int centerRow, int centerCol, DamageSource source)
        {
            // Check all 8 adjacent cells (including diagonals)
            for (int dr = -1; dr <= 1; dr++)
            {
                for (int dc = -1; dc <= 1; dc++)
                {
                    if (dr == 0 && dc == 0) continue; // Skip center cell
                    
                    int targetRow = centerRow + dr;
                    int targetCol = centerCol + dc;
                    CheckCellDamage(targetRow, targetCol, source);
                }
            }
        }
        
        private void CheckCellDamage(int row, int col, DamageSource source)
        {
            if (_grid.TryGet(row, col, out var block) && block != null)
            {
                // Check if the block has a health component (is an obstacle)
                if (block.Type == BlockType.Box)
                {
                    // Find the box behavior and damage it
                    var boxBehavior = FindBoxBehavior(block);
                    if (boxBehavior != null)
                    {
                        boxBehavior.TakeDamage(1, source);
                    }
                }
            }
        }
        
        private IHealthComponent FindBoxBehavior(BlockModel block)
        {
            // This is a simplified approach - in a real implementation,
            // you might want to store the health component reference in the BlockModel
            // or use a different approach to find the health component
            
            // For now, we'll fire a damage event and let the box behavior handle it
            _events.Fire(new BlockDamagedEvent(block, 1, DamageSource.Blast));
            return null; // The box behavior will handle the actual damage
        }
        
        public void OnDestroy()
        {
            _events.Unsubscribe<ClearBlockEvent>(OnBlockCleared);
            _events.Unsubscribe<BlockActivatedEvent>(OnBlockActivated);
        }
    }
}
