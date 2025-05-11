using System.Collections.Generic;
using _Game.Core.Constants;
using _Game.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Game.Systems.GameLoop
{
    public class LevelManager : MonoBehaviour
    {
        [Tooltip("Assign in build order.")]
        [SerializeField] private List<LevelData> allLevels;
        private int _currentIndex;
        public int CurrentLevelIndex => _currentIndex = PlayerPrefs.GetInt(GameConstants.PlayerPrefsLevel, 0);

        public LevelData CurrentLevel =>
            (_currentIndex >= 0 && _currentIndex < allLevels.Count)
                ? allLevels[_currentIndex]
                : null;

        void Awake()
        {
            _currentIndex = CurrentLevelIndex;
            // loop through levels
            
            _currentIndex = (_currentIndex - 1 + allLevels.Count) % allLevels.Count;
            DontDestroyOnLoad(this);
            
        }

        public bool LoadNext()
        {
            if (_currentIndex + 1 >= allLevels.Count) return false;
            _currentIndex++;
            PlayerPrefs.SetInt(GameConstants.PlayerPrefsLevel, _currentIndex);
            PlayerPrefs.Save();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return true;
        }

        public void Reload() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        public bool LoadLevel(int index)
        {
            if (index < 0 || index >= allLevels.Count) return false;
            _currentIndex = index;
            PlayerPrefs.SetInt(GameConstants.PlayerPrefsLevel, _currentIndex);
            PlayerPrefs.Save();
            return true;
        }

        public void RegisterEvents(IEventBus events)
        {
            // events.Subscribe<LevelInitializedEvent>(OnLevelInitialized);
        }
    }
}