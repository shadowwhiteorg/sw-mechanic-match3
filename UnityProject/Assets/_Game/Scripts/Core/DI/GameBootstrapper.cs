using _Game.Core.Events;
using _Game.Interfaces;
using UnityEngine;

namespace _Game.Core.DI
{
    public class GameBootstrapper : MonoBehaviour 
    {
        
        private void Awake() {
            var container = new DIContainer();
        
            // Core Services
            container.BindSingleton<IEventBus>(new EventBus());
            // var exampleManager = Instantiate(examplePrefab).GetComponent<ExampleManager>();
            // container.BindSingleton<ExampleManager>(exampleManager);
            
            // Gameplay
            // container.Bind<IExampleService, ExampleService>();
        
            // Resolve and start
            // var gameManager = container.Resolve<GameManager>();
        }
    }
}