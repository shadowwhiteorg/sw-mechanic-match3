using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _Game.Core.Events;
using _Game.Interfaces;
using _Game.Systems.BlockSystem;
using _Game.Utils;

namespace _Game.Systems.MatchSystem
{
    /// <summary>
    /// Plays a block-specific SFX on clear, using pooled AudioSources.
    /// </summary>
    public class ClearSfxService
    {
        private readonly IEventBus _events;
        readonly BlockTypeConfig _config;
        readonly GridWorldHelper _helper;
        readonly IDictionary<AudioClip, GameObjectPool> _pools;
        readonly CoroutineRunner _runner;

        public ClearSfxService(
            IEventBus events,
            BlockTypeConfig config,
            GridWorldHelper helper,
            IDictionary<AudioClip, GameObjectPool> pools,
            CoroutineRunner runner)
        {
            _events  = events;
            _config  = config;
            _helper  = helper;
            _pools   = pools;
            _runner  = runner;

            _events.Subscribe<ClearBlockEvent>(OnBlockCleared);
        }

        private void OnBlockCleared(ClearBlockEvent e)
        {
            var block = e.Block;
            var entry = _config.Get(block.Color, block.Type);
            var clip  = entry.ClearSfxClip;
            if (clip == null || !_pools.TryGetValue(clip, out var pool))
                return;

            // Get a pooled AudioSource GameObject
            var go = pool.Get();
            go.transform.position = _helper.GetWorldPosition(block.Row, block.Column);

            var src = go.GetComponent<AudioSource>();
            src.clip = clip;
            src.Play();

            // Return to pool after clip duration
            _runner.StartCoroutine(ReturnAfter(go, pool, clip.length));
        }

        private IEnumerator ReturnAfter(GameObject go, GameObjectPool pool, float delay)
        {
            yield return new WaitForSeconds(delay);
            pool.Return(go);
        }
    }
}