using _Game.Core.Events;
using _Game.Interfaces;
using UnityEngine;

namespace _Game.Core.DI
{
    public class GameBootstrapper : MonoBehaviour
    {
        [SerializeField] private SystemRunnerDriver runnerDriver;

        private void Awake()
        {
            var container = new DIContainer();

            // Core Services
            container.BindSingleton<IEventBus>(new EventBus());
            
            var runner = new SystemRunner();
            container.BindSingleton<ISystemRunner>(runner);
            runnerDriver.Initialize(runner);

            // Gameplay
            // container.Bind<IExampleService, ExampleService>();

            // Resolve and start
            // var gameManager = container.Resolve<GameManager>();
        }
    }
}