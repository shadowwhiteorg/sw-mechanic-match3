using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _Game.Core.Events;
using _Game.Interfaces;
using _Game.Systems.BlockSystem;
using _Game.Utils;
using CoroutineRunner = Unity.VisualScripting.CoroutineRunner;

namespace _Game.Systems.BehaviorSystem
{
    [CreateAssetMenu(menuName = "Blast/Behaviors/Rocket")]
    public class RocketBehaviorAsset : BlockBehaviorAsset
    {
        [SerializeField] private GameObject _trailPrefab;
        [SerializeField] private int _trailPoolSize = 8;
        [SerializeField] private float _perCellDelay = 0.05f;

        private GameObjectPool _trailPool;
        private IGridHandler _grid;
        private IEventBus _events;
        private Transform _origin;

        public override void Initialize(
            IGridHandler grid, IBlockFactory factory, GridWorldHelper helper, IEventBus eventBus)
        {
            base.Initialize(grid, factory, helper, eventBus);
            _grid = grid;
            _events = eventBus;
            // _origin = helper.OriginTransform; // where trails live in the hierarchy
            if (_trailPrefab)
                _trailPool = new GameObjectPool(_trailPrefab, _trailPoolSize, _origin);
        }

        public override void OnActivated(BlockModel block)
        {
            Launch(block);
        }
        
        public override void OnCleared(BlockModel block)
        {
        }
        
        private void Launch(BlockModel block)
        {
            // decide axis & rotate block.view…
            bool horizontal = Random.value < 0.5f;
            block.View.transform.rotation = Quaternion.Euler(0, 0, horizontal ? 0f : 90f);
            Vector2 axis = horizontal ? Vector2.right : Vector2.up;

            // launch two trails:
            SpawnTrail(block, axis, +1);
            SpawnTrail(block, axis, -1);

            // then sweep & clear
            CoroutineRunner.instance.StartCoroutine(SweepAndClear(block, axis));
        }

        private void SpawnTrail(BlockModel block, Vector2 axis, int sign)
        {
            if (_trailPool == null) return;
            var go = _trailPool.Get();
            go.transform.SetParent(_origin, false);
            go.transform.position = block.View.transform.position;
            float ang = (axis == Vector2.right) ? 0f : 90f;
            go.transform.rotation = Quaternion.Euler(0, 0, ang);

            CoroutineRunner.instance.StartCoroutine(AnimateTrail(go, axis * (sign * 3f)));
        }

        private IEnumerator AnimateTrail(GameObject go, Vector3 offset)
        {
            Vector3 start = go.transform.position;
            Vector3 end = start + offset;
            float t = 0f, dur = 0.3f;

            while (t < dur)
            {
                go.transform.position = Vector3.Lerp(start, end, t / dur);
                t += Time.deltaTime;
                yield return null;
            }

            go.transform.position = end;
            _trailPool.Return(go);
        }

        private IEnumerator SweepAndClear(BlockModel block, Vector2 axis)
        {
            int max = axis == Vector2.right ? _grid.Columns : _grid.Rows;
            var blocksToRemove = new List<BlockModel>();
            block.View.gameObject.SetActive(false);
            for (int i = 1; i < max; i++)
            {
                yield return new WaitForSeconds(_perCellDelay);

                int r = block.Row + (int)(axis.y * i);
                int c = block.Column + (int)(axis.x * i);
                if (_grid.TryGet(r, c, out var t))
                {
                    t.View.gameObject.SetActive(false);
                    blocksToRemove.Add(t);
                    // _events.Fire(new ClearBlockEvent(t));
                }

                r = block.Row - (int)(axis.y * i);
                c = block.Column - (int)(axis.x * i);
                if (_grid.TryGet(r, c, out t))
                {
                    t.View.gameObject.SetActive(false);
                    blocksToRemove.Add(t);
                    // _events.Fire(new ClearBlockEvent(t));
                }
            }

            _events.Fire(new ClearBlockEvent(block));
            foreach (var blk in blocksToRemove)
                _events.Fire(new ClearBlockEvent(blk));
            
        }

       
    }
}
