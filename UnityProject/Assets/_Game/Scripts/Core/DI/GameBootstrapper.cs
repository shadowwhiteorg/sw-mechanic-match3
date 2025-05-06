using _Game.Core.Events;
using _Game.Interfaces;
using UnityEngine;

namespace _Game.Core.DI
{
    public class GameBootstrapper : MonoBehaviour
    {
        [SerializeField] private GameInstaller gameInstaller;
        [SerializeField] private SystemRunnerDriver runnerDriver;

        private void Awake()
        {

            // Core Services
            var container = new DIContainer();
            container.BindSingleton<IEventBus>(new EventBus());
            
            //Gameplay
            var runner = new SystemRunner();
            
            container.BindSingleton<ISystemRunner>(runner);
            runnerDriver.Initialize(runner);
            
            gameInstaller.Initialize();
        }
    }
}