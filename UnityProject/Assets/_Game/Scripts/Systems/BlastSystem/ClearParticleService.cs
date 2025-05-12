using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using _Game.Core.Events;
using _Game.Interfaces;
using _Game.Systems.BlockSystem;
using _Game.Utils;

namespace _Game.Systems.MatchSystem
{
    /// <summary>
    /// Spawns block-specific particles on clear, using per-prefab pools
    /// and a CoroutineRunner for delayed return.
    /// </summary>
    public class ClearParticleService
    {
        private readonly BlockTypeConfig                          _config;
        private readonly GridWorldHelper                          _helper;
        private readonly IDictionary<ParticleSystem, GameObjectPool> _pools;

        public ClearParticleService(
            IEventBus events,
            BlockTypeConfig config,
            GridWorldHelper helper,
            IDictionary<ParticleSystem, GameObjectPool> pools)
        {
            _config  = config;
            _helper  = helper;
            _pools   = pools;

            events.Subscribe<ClearBlockEvent>(OnBlockCleared);
        }

        private void OnBlockCleared(ClearBlockEvent e)
        {
            var block = e.Block;
            var entry = _config.Get(block.Color, block.Type);
            var psPrefab = entry.ClearParticlePrefab;
            if (psPrefab == null) return;                 // no particle for this block
            if (!_pools.TryGetValue(psPrefab, out var pool)) return;

            // Get a pooled GameObject, position & play
            var go = pool.Get();
            go.transform.position = _helper.GetWorldPosition(block.Row, block.Column);

            var ps = go.GetComponent<ParticleSystem>();
            ps.Play(true);

            // schedule return after its lifetime
            var main      = ps.main;
            float duration = main.duration + main.startLifetime.constant;
            CoroutineRunner.Instance.StartCoroutine(ReturnAfter(go, pool, duration));
        }

        private IEnumerator ReturnAfter(GameObject go, GameObjectPool pool, float delay)
        {
            yield return new WaitForSeconds(delay);
            pool.Return(go);
        }
    }
}
