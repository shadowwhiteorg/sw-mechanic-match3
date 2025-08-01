﻿using UnityEngine;
using UnityEngine.SceneManagement;
using _Game.Core.Constants;
using _Game.Core.Data;

namespace _Game.Systems.GameLoop
{

    public class LevelManager : MonoBehaviour
    {
        [Tooltip("Reference the LevelLibrary asset containing JSON TextAssets in build order.")]
        [SerializeField] private LevelLibrary _levelLibrary;

        private int       _currentIndex;
        private LevelData _currentLevelData;
        
        public int CurrentLevelIndex => _currentIndex = PlayerPrefs.GetInt(GameConstants.PlayerPrefsLevel, 0);

        
        public LevelData CurrentLevel => _currentLevelData;

        public void Initialize()
        {
            // 1. Read the saved index (default to 0)
            _currentIndex = PlayerPrefs.GetInt(GameConstants.PlayerPrefsLevel, 0);

            // 2. Validate the library
            if (_levelLibrary == null || _levelLibrary.levelJsonAssets.Count == 0)
            {
                Debug.LogError("[LevelManager] LevelLibrary is null or empty!");
                return;
            }

            // 3. Wrap the index into valid range
            int count = _levelLibrary.levelJsonAssets.Count;
            var index = (_currentIndex % count + count) % count;

            // 4. Load and clone the JSON TextAsset
            var jsonAsset = _levelLibrary.levelJsonAssets[index];
            _currentLevelData = LevelDataFileHandler.Load(jsonAsset);
            if (_currentLevelData == null)
                Debug.LogError($"[LevelManager] Failed to load LevelData for index {_currentIndex}");
        }


        public void LoadLevelScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }


        public void LevelUp()
        {
            if (_levelLibrary == null || _levelLibrary.levelJsonAssets.Count == 0)
                return;
            _currentIndex++;
            PlayerPrefs.SetInt(GameConstants.PlayerPrefsLevel, _currentIndex);
            PlayerPrefs.Save();
        }
    }
}
