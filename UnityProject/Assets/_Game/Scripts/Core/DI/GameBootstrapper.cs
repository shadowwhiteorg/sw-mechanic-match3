using _Game.Core.Events;
using _Game.Interfaces;
using _Game.Scripts.Interfaces;
using _Game.Systems;
using _Game.Systems.GridSystem;
using _Game.Systems.MatchSystem;
using _Game.Utils;
using UnityEngine;

namespace _Game.Core.DI
{
    public class GameBootstrapper : MonoBehaviour
    {
        [SerializeField] private GameInstaller gameInstallerPrefab;
        [SerializeField] private SystemRunnerDriver runnerDriverPrefab;

        private void Awake()
        {

            // Core Services
            var container = new DIContainer();
            
            var eventBus = new EventBus();
            container.BindSingleton<IEventBus>(eventBus);
            
            var gameInstaller = Instantiate(gameInstallerPrefab);
            gameInstaller.Initialize(container);
            
            //Gameplay
            var runner = new SystemRunner();
            container.BindSingleton<ISystemRunner>(runner);
            var runnerDriver = Instantiate(runnerDriverPrefab);
            runnerDriver.Initialize(runner);
            
            var inputSystem = new InputSystem(Camera.main, container.Resolve<GridWorldHelper>(), container.Resolve<IEventBus>());
            container.BindSingleton<IUpdatableSystem>(inputSystem);
            runner.Register(inputSystem);
            
            var matchSys = new MatchSystem(container.Resolve<IGridHandler>(), eventBus);
            container.BindSingleton(matchSys);
            
            
        }
    }
}