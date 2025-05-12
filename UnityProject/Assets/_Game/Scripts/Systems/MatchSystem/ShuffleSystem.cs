using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Game.Core.Events;
using _Game.Enums;
using _Game.Interfaces;
using _Game.Systems.BlockSystem;
using _Game.Systems.GridSystem;
using _Game.Utils;
using UnityEngine;

namespace _Game.Systems.GameLoop
{
    public class ShuffleSystem : IUpdatableSystem
    {
        private readonly IGridHandler _grid;
        private readonly int _matchThreshold;
        private readonly IBlockFactory _blockFactory;
        private readonly GridWorldHelper _gridHelper;
        private readonly System.Random _rng = new();

        public ShuffleSystem(
            IGridHandler grid,
            IEventBus eventBus,
            GridConfig gridConfig,
            IBlockFactory blockFactory,
            GridWorldHelper gridHelper)
        {
            _grid = grid;
            _matchThreshold = gridConfig.MatchThreshold;
            _blockFactory = blockFactory;
            _gridHelper = gridHelper;

            eventBus.Subscribe<TurnEndedEvent>(e=>OnTurnEnded());
            eventBus.Subscribe<LevelInitializedEvent>(e=>OnTurnEnded());
        }

        public void Tick() { }

        private void OnTurnEnded()
        {
            if (HasPossibleMatches()) return;
            CoroutineRunner.Instance.StartCoroutine(ShuffleRoutine());
        }

        private bool HasPossibleMatches()
        {
            for (int r = 0; r < _grid.Rows; r++)
            {
                for (int c = 0; c < _grid.Columns; c++)
                {
                    var block = _grid.GetBlock(r, c);
                    if (block == null || block.Type != BlockType.None) continue;

                    var group = DetectGroup(r, c, block.Color);
                    if (group.Count >= _matchThreshold) return true;
                }
            }
            return false;
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

            while (queue.Count > 0)
            {
                var (r, c) = queue.Dequeue();
                if (visited[r][c]) continue;
                visited[r][c] = true;

                var block = _grid.GetBlock(r, c);
                if (block == null || block.Color != color) continue;

                result.Add(block);
                foreach (var (nr, nc) in new[] { (r - 1, c), (r + 1, c), (r, c - 1), (r, c + 1) })
                {
                    if (_grid.IsInside(nr, nc) && !visited[nr][nc])
                        queue.Enqueue((nr, nc));
                }
            }
            return result;
        }

        private IEnumerator ShuffleRoutine()
        {
            // Collect regular blocks and their positions
            var regularBlocks = new List<BlockModel>();
            var positions = new List<Vector2Int>();

            for (int r = 0; r < _grid.Rows; r++)
            {
                for (int c = 0; c < _grid.Columns; c++)
                {
                    var block = _grid.GetBlock(r, c);
                    if (block != null && block.Type == BlockType.None)
                    {
                        regularBlocks.Add(block);
                        positions.Add(new Vector2Int(r, c));
                    }
                }
            }

            // Determine target color
            var targetColor = regularBlocks
                .GroupBy(b => b.Color)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();

            // Force a horizontal group
            if (TryForceGroup(regularBlocks, targetColor, positions, out var forcedMappings))
            {
                // Map remaining blocks to shuffled positions
                var remainingPositions = positions.Except(forcedMappings.Values).ToList();
                var remainingBlocks = regularBlocks.Except(forcedMappings.Keys).ToList();
                remainingPositions = remainingPositions.OrderBy(_ => _rng.Next()).ToList();

                var allMappings = forcedMappings.Concat(remainingBlocks
                    .Select((b, i) => new KeyValuePair<BlockModel, Vector2Int>(b, remainingPositions[i])))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                // Animate movements
                yield return AnimateShuffle(allMappings);
                _grid.ClearAllColoredBlocks();
                // Update grid positions
                foreach (var (block, newPos) in allMappings)
                {

                    _grid.SetBlock(newPos.x, newPos.y, block);
                }

                // _eventBus.Fire(new TurnEndedEvent());
            }
        }

        private bool TryForceGroup(
            List<BlockModel> blocks,
            BlockColor targetColor,
            List<Vector2Int> allPositions,
            out Dictionary<BlockModel, Vector2Int> mappings)
        {
            mappings = new Dictionary<BlockModel, Vector2Int>();
            var targetBlocks = blocks.Where(b => b.Color == targetColor).ToList();

            if (targetBlocks.Count < _matchThreshold) return false;

            // Find valid horizontal position
            for (int attempt = 0; attempt < 100; attempt++)
            {
                int row = _rng.Next(0, _grid.Rows);
                int col = _rng.Next(0, _grid.Columns - (_matchThreshold - 1));

                var candidatePositions = Enumerable.Range(col, _matchThreshold)
                    .Select(c => new Vector2Int(row, c))
                    .Where(p => allPositions.Contains(p))
                    .ToList();

                if (candidatePositions.Count == _matchThreshold)
                {
                    for (int i = 0; i < _matchThreshold; i++)
                    {
                        mappings[targetBlocks[i]] = candidatePositions[i];
                        allPositions.Remove(candidatePositions[i]);
                    }
                    return true;
                }
            }
            return false;
        }

        private IEnumerator AnimateShuffle(Dictionary<BlockModel, Vector2Int> mappings)
        {
            var animations = new List<Coroutine>();
            foreach (var (block, newPos) in mappings)
            {
                Vector3 targetPos = _gridHelper.GetWorldPosition(newPos.x, newPos.y);
                animations.Add(Tween.Position(
                    block.View.transform,
                    targetPos,
                    0.5f,
                    Ease.OutQuad
                ));
            }

            // Wait for all animations to complete
            foreach (var anim in animations)
                yield return anim;
        }
    }
}