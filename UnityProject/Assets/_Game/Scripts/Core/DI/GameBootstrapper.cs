using UnityEngine;
using _Game.Interfaces;
using _Game.Core.Events;
using _Game.Utils;
using _Game.Systems.GameLoop;
using _Game.Systems.UISystem;

namespace _Game.Core.DI
{
    public class GameBootstrapper : MonoBehaviour
    {
        [Header("Prefabs & Configs")]
        [SerializeField] private GameInstaller installerPrefab;
        [SerializeField] private UIInstaller uiInstaller;
        [SerializeField] private SystemRunnerDriver runnerDriverPrefab;
        [SerializeField] private LevelManager levelManager;
        [SerializeField] private LevelData  fallbackLevel;

        private void Awake()
        {
            var container = new DIContainer();
            var eventBus  = new EventBus();
            container.BindSingleton<IEventBus>(eventBus);
            container.BindSingleton(container);
            container.BindSingleton(CoroutineRunner.Instance);

            // System runner
            var runner = new SystemRunner();
            container.BindSingleton<ISystemRunner>(runner);
            var driver = Instantiate(runnerDriverPrefab);
            driver.Initialize(runner);

            // Level Manager & LevelData
            container.BindSingleton(levelManager);
            var levelData = levelManager.CurrentLevel ?? fallbackLevel;
            container.BindSingleton(levelData);

            // Initialize core systems
            var installer = Instantiate(installerPrefab);
            installer.Initialize(container, eventBus, levelManager);
            // var uiInstaller = Instantiate(uiInstallerPrefab);
            uiInstaller.Initialize(container, eventBus);

            // Load level
            var loader = new LevelLoader(container,eventBus,levelManager);
            eventBus.Fire(new LevelInitializedEvent(levelManager.CurrentLevelIndex,levelManager.CurrentLevel));
        }
    }
}