// Core/DI/GameBootstrapper.cs

using System.Collections.Generic;
using UnityEngine;
using _Game.Interfaces;
using _Game.Core.Events;
using _Game.Utils;
using _Game.Systems.CoreSystems;
using _Game.Systems.MatchSystem;
using _Game.Systems.GridSystem;

namespace _Game.Core.DI
{
    public class GameBootstrapper : MonoBehaviour
    {
        [SerializeField] private GameInstaller installerPrefab;
        [SerializeField] private SystemRunnerDriver runnerDriverPrefab;

        private void Awake()
        {
            // Core DI container & event bus
            var container = new DIContainer();
            var eventBus = new EventBus();
            container.BindSingleton<IEventBus>(eventBus);

            // Install core game systems
            var installer = Instantiate(installerPrefab);
            installer.Initialize(container, eventBus);

            // Runner and driver
            var runner = new SystemRunner();
            container.BindSingleton<ISystemRunner>(runner);

            var driver = Instantiate(runnerDriverPrefab);
            driver.Initialize(runner);
            

            // InputSystem is registered manually
            var input = new InputSystem(Camera.main, container.Resolve<GridWorldHelper>(), eventBus);
            container.BindSingleton<IUpdatableSystem>(input);
            runner.Register(input);
        }
    }
}