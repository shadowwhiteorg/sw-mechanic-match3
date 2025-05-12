using _Game.Core.DI;
using _Game.Interfaces;
using _Game.Systems.BlockSystem;
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

        [SerializeField] private Canvas canvasRoot;
        [SerializeField] private BlockTypeConfig blockTypeConfig;

        public void Initialize(DIContainer container, IEventBus eventBus)
        {
            InstallGoalUI(container, eventBus);
            InstallWinUI(container, eventBus);
            InstallLoseUI(container, eventBus);
            InstallMoveUI(container, eventBus);
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