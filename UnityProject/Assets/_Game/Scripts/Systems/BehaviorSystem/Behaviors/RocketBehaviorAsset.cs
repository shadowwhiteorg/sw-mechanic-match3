using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _Game.Core.Events;
using _Game.Enums;
using _Game.Interfaces;
using _Game.Systems.BlockSystem;
using _Game.Utils;
using CoroutineRunner = Unity.VisualScripting.CoroutineRunner;

namespace _Game.Systems.BehaviorSystem
{
    [CreateAssetMenu(menuName = "Blast/Behaviors/Rocket")]
    public class RocketBehaviorAsset : BlockBehaviorAsset
    {
        [SerializeField] private GameObject trailPrefab;
        [SerializeField] private int trailPoolSize = 8;
        [SerializeField] private float perCellDelay = 0.05f;

        private GameObjectPool _trailPool;
        private Transform _origin;

        public override void Initialize(
            IGridHandler grid, IBlockFactory factory, GridWorldHelper helper, IEventBus eventBus)
        {
            base.Initialize(grid, factory, helper, eventBus);
            if (trailPrefab)
                _trailPool = new GameObjectPool(trailPrefab, trailPoolSize, _origin);
        }

        public override void OnActivated(BlockModel block)
        {
            base.OnActivated(block);
            Events.Fire(new BlockActivatedEvent());
            Launch(block);
        }
        
        private void Launch(BlockModel block)
        {
            bool horizontal = Random.value < 0.5f;
            block.View.transform.rotation = Quaternion.Euler(0, 0, horizontal ? 0f : 90f);
            Vector2 axis = horizontal ? Vector2.right : Vector2.up;

            SpawnTrail(block, axis, +1);
            SpawnTrail(block, axis, -1);

            CoroutineRunner.instance.StartCoroutine(SweepAndClear(block, axis));
        }

        private void SpawnTrail(BlockModel block, Vector2 axis, int sign)
        {
            if (_trailPool == null) return;
            var go = _trailPool.Get();
            go.transform.SetParent(null, false);
            go.transform.position = block.View.transform.position;
            float ang = (axis == Vector2.right) ? 0f : 90f;
            go.transform.rotation = Quaternion.Euler(0, 0, ang);

            CoroutineRunner.instance.StartCoroutine(AnimateTrail(go, axis * (sign * 15f)));
        }

        private IEnumerator AnimateTrail(GameObject go, Vector3 offset)
        {
            Vector3 start = go.transform.position;
            Vector3 end = start + offset;
            float t = 0f, dur = 0.75f;

            while (t < dur)
            {
                go.transform.position = Vector3.Lerp(start, end, t / dur);
                t += Time.deltaTime;
                yield return null;
            }

            go.transform.position = end;
            yield return new WaitForSeconds(2f);
            _trailPool.Return(go);
        }

        private IEnumerator SweepAndClear(BlockModel block, Vector2 axis)
        {
            int max = axis == Vector2.right ? Grid.Columns : Grid.Rows;
            var blocksToRemove = new List<BlockModel>();
            block.View.gameObject.SetActive(false);
            for (int i = 1; i < max; i++)
            {
                yield return new WaitForSeconds(perCellDelay);

                int r = block.Row + (int)(axis.y * i);
                int c = block.Column + (int)(axis.x * i);
                if (Grid.TryGet(r, c, out var t))
                {
                    if (t.CanClear())
                    {
                        t.View.gameObject.SetActive(false);
                        blocksToRemove.Add(t);
                    }
                }

                r = block.Row - (int)(axis.y * i);
                c = block.Column - (int)(axis.x * i);
                if (Grid.TryGet(r, c, out t))
                {
                    if (t.CanClear())
                    {
                        t.View.gameObject.SetActive(false);
                        blocksToRemove.Add(t);
                    }
                }
            }
            
            Events.Fire(new BlockDeactivatedEvent());
            Events.Fire(new ClearBlockEvent(block));
            foreach (var blk in blocksToRemove)
            {
                if (blk.Type != BlockType.None)
                    Events.Fire(new BlockSelectedEvent(blk.Row, blk.Column));
                else
                    Events.Fire(new ClearBlockEvent(blk));
            }
        }
    }
}
