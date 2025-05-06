using _Game.Core.Events;
using _Game.Interfaces;
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
            container.BindSingleton<IEventBus>(new EventBus());
            
            //Gameplay
            var runner = new SystemRunner();
            container.BindSingleton<ISystemRunner>(runner);
            var runnerDriver = Instantiate(runnerDriverPrefab);
            runnerDriver.Initialize(runner);
            
            // Game Installer
            var gameInstaller = Instantiate(gameInstallerPrefab);
            gameInstaller.Initialize();
        }
    }
}