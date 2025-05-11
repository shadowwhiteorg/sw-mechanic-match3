using _Game.Core.DI;
using _Game.Interfaces;
using _Game.Systems.BlockSystem;
using UnityEngine;

namespace _Game.Systems.UISystem
{
    public class UIInstaller : MonoBehaviour
    {
        [Header("UI References")] 
        [SerializeField] private GoalUIScreen goalUIScreenPrefab;
        [SerializeField] private WinUIScreen winUIScreenPrefab;
        [SerializeField] private LoseUIScreen loseUIScreenPrefab;

        [SerializeField] private Canvas canvasRoot;
        [SerializeField] private BlockTypeConfig blockTypeConfig;

        public void Initialize(DIContainer container, IEventBus eventBus)
        {
            InstallGoalUI(container, eventBus);
            InstallWinUIScreen(container, eventBus);
            InstallLoseUIScreen(container, eventBus);
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
            Debug.Log("GoalUIScreen: " + screen + " View: " + view);
            // BIND TO SCREEN
            screen.Construct(model, view, eventBus);
            canvasRoot.worldCamera = Camera.main;
        }

        private void InstallWinUIScreen(DIContainer container, IEventBus eventBus)
        {
            // MODEL
            var model = new WinUIModel();
            container.BindSingleton(model);
            // PREFAB
            var screenGO = Instantiate(winUIScreenPrefab, canvasRoot.transform);
            var view = screenGO.GetComponentInChildren<WinUIView>();
            var screen = screenGO.GetComponent<WinUIScreen>();
            Debug.Log("WinUIScreen: " + screen + " View: " + view);
            // BIND TO SCREEN
            screen.Construct(model, view, eventBus);
        }
        private void InstallLoseUIScreen(DIContainer container, IEventBus eventBus)
        {
            // MODEL
            var model = new LoseUIModel();
            container.BindSingleton(model);
            // PREFAB
            var screenGO = Instantiate(loseUIScreenPrefab, canvasRoot.transform);
            var view = screenGO.GetComponentInChildren<LoseUIView>();
            var screen = screenGO.GetComponent<LoseUIScreen>();
            Debug.Log("LoseUIScreen: " + screen + " View: " + view);
            // BIND TO SCREEN
            screen.Construct(model, view, eventBus);
        }
    }
}