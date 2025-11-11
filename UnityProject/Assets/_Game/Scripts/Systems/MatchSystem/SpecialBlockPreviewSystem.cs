using System.Collections.Generic;
using UnityEngine;
using _Game.Core.Events;
using _Game.Enums;
using _Game.Interfaces;
using _Game.systems.BlockSystem;
using _Game.Systems.BlockSystem;
using _Game.Systems.GridSystem;
using _Game.Utils;

namespace _Game.Systems.MatchSystem
{
    public class SpecialBlockPreviewSystem : IUpdatableSystem
    {
        private readonly IEventBus _events;
        private readonly SpecialBlockSpawnConfig _spawnConfig;
        private readonly BlockTypeConfig _blockTypeConfig;
        private readonly IGridHandler _grid;
        private readonly GridWorldHelper _gridHelper;
        
        private Dictionary<BlockModel, Sprite> _originalSprites = new();
        private List<BlockModel> _previewBlocks = new();
        private bool _isPreviewActive = false;
        private bool _needsRescan = true;

        public SpecialBlockPreviewSystem(IEventBus events, SpecialBlockSpawnConfig spawnConfig, BlockTypeConfig blockTypeConfig, IGridHandler grid, GridWorldHelper gridHelper)
        {
            _events = events;
            _spawnConfig = spawnConfig;
            _blockTypeConfig = blockTypeConfig;
            _grid = grid;
            _gridHelper = gridHelper;
            
            Debug.Log("[SpecialBlockPreview] System initialized");
            
            // Subscribe to events
            _events.Subscribe<BlockSelectedEvent>(OnBlockSelected);
            _events.Subscribe<MatchFoundEvent>(OnMatchFound);
            _events.Subscribe<ClearBlockEvent>(OnBlockCleared);
            _events.Subscribe<TurnEndedEvent>(OnTurnEnded);
            
            Debug.Log("[SpecialBlockPreview] Event subscriptions completed");
            
            // Start with an initial scan
            _needsRescan = true;
        }

        public void Tick() 
        {
            // Only scan when needed to avoid performance issues
            if (_needsRescan)
            {
                ScanBoardForPreviews();
                _needsRescan = false;
            }
        }

        private void ScanBoardForPreviews()
        {
            Debug.Log("[SpecialBlockPreview] Scanning board for potential special block matches");
            
            // Clear any existing previews
            ClearPreview();
            
            // Scan the entire board for potential matches
            for (int row = 0; row < _grid.Rows; row++)
            {
                for (int col = 0; col < _grid.Columns; col++)
                {
                    if (_grid.TryGet(row, col, out var block) && block.Type == BlockType.None)
                    {
                        var group = DetectGroup(row, col, block.Color);
                        if (group.Count >= 3) // Check if it's a valid match
                        {
                            var specialType = _spawnConfig.GetTypeForMatch(group.Count);
                            if (specialType != BlockType.None)
                            {
                                Debug.Log($"[SpecialBlockPreview] Found potential special block match: {group.Count} blocks will create {specialType}");
                                ShowSpecialBlockPreview(group, specialType);
                                return; // Only show one preview at a time for now
                            }
                        }
                    }
                }
            }
        }

        // Test method to verify the system is working
        public void TestPreviewSystem()
        {
            Debug.Log("[SpecialBlockPreview] Test method called - system is working!");
        }

        private void OnBlockSelected(BlockSelectedEvent e)
        {
            Debug.Log($"[SpecialBlockPreview] Block selected at ({e.Row}, {e.Col}) - marking for rescan");
            
            // Mark for rescan when a block is selected
            _needsRescan = true;
        }

        private void OnMatchFound(MatchFoundEvent e)
        {
            Debug.Log($"[SpecialBlockPreview] Match found with {e.Blocks.Count} blocks - clearing previews");
            
            // Clear previews when a match is found
            ClearPreview();
            _needsRescan = true;
        }

        private void OnBlockCleared(ClearBlockEvent e)
        {
            Debug.Log($"[SpecialBlockPreview] Block cleared - marking for rescan");
            
            // Mark for rescan when blocks are cleared
            _needsRescan = true;
        }

        private void OnTurnEnded(TurnEndedEvent e)
        {
            Debug.Log($"[SpecialBlockPreview] Turn ended - marking for rescan");
            
            // Mark for rescan when turn ends
            _needsRescan = true;
        }

        private void ShowSpecialBlockPreview(List<BlockModel> blocks, BlockType specialType)
        {
            Debug.Log($"[SpecialBlockPreview] ShowSpecialBlockPreview called for {specialType}");
            
            if (_isPreviewActive) 
            {
                Debug.Log("[SpecialBlockPreview] Preview already active, skipping");
                return;
            }
            
            // Get the preview sprite for this special block type
            var configEntry = _blockTypeConfig.Get(BlockColor.None, specialType);
            Debug.Log($"[SpecialBlockPreview] Config entry found: {configEntry.PreviewSprite != null}");
            Debug.Log($"[SpecialBlockPreview] Special type: {specialType}, Preview sprite: {configEntry.PreviewSprite}");
            
            if (configEntry.PreviewSprite == null) 
            {
                Debug.LogWarning($"[SpecialBlockPreview] No preview sprite found for {specialType}. Please assign a preview sprite in BlockTypeConfig!");
                return;
            }
            
            _isPreviewActive = true;
            _previewBlocks.Clear();
            _originalSprites.Clear();

            Debug.Log($"[SpecialBlockPreview] Setting preview sprites for {blocks.Count} blocks");
            foreach (var block in blocks)
            {
                if (block.View != null)
                {
                    // Store original sprite
                    var spriteRenderer = block.View.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null)
                    {
                        _originalSprites[block] = spriteRenderer.sprite;
                        
                        // Set preview sprite
                        block.View.SetSprite(configEntry.PreviewSprite);
                        _previewBlocks.Add(block);
                        Debug.Log($"[SpecialBlockPreview] Set preview sprite for block at ({block.Row}, {block.Column})");
                    }
                }
            }
        }

        private void ClearPreview()
        {
            if (!_isPreviewActive) return;
            
            _isPreviewActive = false;

            // Restore original sprites
            foreach (var block in _previewBlocks)
            {
                if (_originalSprites.TryGetValue(block, out var originalSprite) && block.View != null)
                {
                    block.View.SetSprite(originalSprite);
                }
            }

            _previewBlocks.Clear();
            _originalSprites.Clear();
        }

        private List<BlockModel> DetectGroup(int row, int col, BlockColor color)
        {
            var result = new List<BlockModel>();
            var visited = new bool[_grid.Rows][];
            for (int index = 0; index < _grid.Rows; index++)
            {
                visited[index] = new bool[_grid.Columns];
            }

            var queue = new Queue<(int r, int c)>();
            queue.Enqueue((row, col));
            visited[row][col] = true;

            while (queue.Count > 0)
            {
                var (r, c) = queue.Dequeue();
                var b = _grid.GetBlock(r, c);
                if (b == null || b.Color != color) continue;

                result.Add(b);
                foreach (var (nr, nc) in new[] {(r - 1, c), (r + 1, c), (r, c - 1), (r, c + 1)})
                {
                    if (_grid.IsInside(nr, nc) && !visited[nr][nc])
                    {
                        visited[nr][nc] = true;
                        queue.Enqueue((nr, nc));
                    }
                }
            }

            return result;
        }
    }
}
