using System.Collections.Generic;
using _Game.Core.DI;
using _Game.Interfaces;
using _Game.Systems.BlockSystem;
using _Game.Systems.MatchSystem;
using _Game.Systems.UISystem.Move;
using _Game.Utils;
using UnityEngine;

namespace _Game.Systems.UISystem
{
    public class UIInstaller : MonoBehaviour
    {
        [Header("UI References")] 
        [SerializeField] private GoalUIScreen goalUIScreenPrefab;
        [SerializeField] private GameObject goalFlyerPrefab;
        [SerializeField] private WinUIScreen winUIScreenPrefab;
        [SerializeField] private LoseUIScreen loseUIScreenPrefab;
        [SerializeField] private MoveUIScreen moveUIScreenPrefab;
        [Header("SFX References")]
        [SerializeField] private GameObject audioSourcePrefab;

        [SerializeField] private Canvas canvasRoot;
        [SerializeField] private BlockTypeConfig blockTypeConfig;

        public void Initialize(DIContainer container, IEventBus eventBus)
        {
            InstallGoalUI(container, eventBus);
            InstallWinUI(container, eventBus);
            InstallLoseUI(container, eventBus);
            InstallMoveUI(container, eventBus);
            InstallClearParticles(eventBus, container);
            InstallClearSFXs(container, eventBus);
        }

        private void InstallGoalUI(DIContainer container, IEventBus eventBus)
        {
            // MODEL
            var model = new GoalUIModel();
            container.BindSingleton(model);
            // PREFAB
            var screenGO = Instantiate(goalUIScreenPrefab, canvasRoot.transform);
            var view = screenGO.GetComponentInChildren<GoalUIView>();
            var screen = screenGO.GetComponent<GoalUIScreen>();
            // BIND TO SCREEN
            screen.Construct(model, view, eventBus);
            canvasRoot.worldCamera = Camera.main;
            
            // ——— Pool & Flyer Service ——
            var helper = container.Resolve<GridWorldHelper>();
            var pool   = new GameObjectPool(goalFlyerPrefab, 5, this.transform);

            var flyerService = new GoalFlyerService(
                eventBus,
                blockTypeConfig,
                helper,
                view,
                model,
                pool
            );
        }

        private void InstallClearParticles(IEventBus eventBus, DIContainer container)
        {
            // 1) Build a pool for each ClearParticlePrefab in your config
            var particlePools = new Dictionary<ParticleSystem, GameObjectPool>();
            foreach (var entry in blockTypeConfig.Entries)  // replace Entries with your actual collection
            {
                var psPrefab = entry.ClearParticlePrefab;
                if (psPrefab != null && !particlePools.ContainsKey(psPrefab))
                {
                    // create a pool of GameObjects (each with a ParticleSystem component)
                    var pool = new GameObjectPool(
                        psPrefab.gameObject,
                         10,
                        parent: this.transform
                    );
                    particlePools[psPrefab] = pool;
                }
            }


            var clearParticleSvc = new ClearParticleService(
                eventBus,
                blockTypeConfig,
                container.Resolve<GridWorldHelper>(),
                particlePools
            );
            container.BindSingleton(clearParticleSvc);
        }

        private void InstallClearSFXs(DIContainer container, IEventBus eventBus)
        {
            var sfxPools = new Dictionary<AudioClip, GameObjectPool>();
            foreach (var entry in blockTypeConfig.Entries)  // or your config’s list
            {
                var clip = entry.ClearSfxClip;
                if (clip != null && !sfxPools.ContainsKey(clip))
                {
                    // audioSourcePrefab is a prefab with an AudioSource component, spatialBlend=0 for 2D
                    var pool = new GameObjectPool(
                        audioSourcePrefab,   // assign in inspector
                        10,
                        parent: this.transform
                    );
                    sfxPools[clip] = pool;
                }
            }

            // 2) Resolve helper and runner
            var helper = container.Resolve<GridWorldHelper>();
            var runner = container.Resolve<CoroutineRunner>();

            // 3) Bind service
            var sfxService = new ClearSfxService(
                eventBus,
                blockTypeConfig,
                helper,
                sfxPools,
                runner
            );
            container.BindSingleton(sfxService);
        }

        private void InstallWinUI(DIContainer container, IEventBus eventBus)
        {
            // MODEL
            var model = new WinUIModel();
            container.BindSingleton(model);
            // PREFAB
            var screenGO = Instantiate(winUIScreenPrefab, canvasRoot.transform);
            var view = screenGO.GetComponentInChildren<WinUIView>();
            var screen = screenGO.GetComponent<WinUIScreen>();
            // BIND TO SCREEN
            screen.Construct(model, view, eventBus);
        }
        private void InstallLoseUI(DIContainer container, IEventBus eventBus)
        {
            // MODEL
            var model = new LoseUIModel();
            container.BindSingleton(model);
            // PREFAB
            var screenGO = Instantiate(loseUIScreenPrefab, canvasRoot.transform);
            var view = screenGO.GetComponentInChildren<LoseUIView>();
            var screen = screenGO.GetComponent<LoseUIScreen>();
            // BIND TO SCREEN
            screen.Construct(model, view, eventBus);
        }
        
        private void InstallMoveUI(DIContainer container, IEventBus eventBus)
        {
            // MODEL
            var model = new MoveUIModel();
            container.BindSingleton(model);
            // PREFAB
            var screenGO = Instantiate(moveUIScreenPrefab, canvasRoot.transform);
            var view = screenGO.GetComponentInChildren<MoveUIView>();
            var screen = screenGO.GetComponent<MoveUIScreen>();
            // BIND TO SCREEN
            screen.Construct(model, view, eventBus);
        }
    }
}