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

        public LevelData CurrentLevel => allLevels[(_currentIndex - 1 + allLevels.Count) % allLevels.Count];

        

        public void LoadLevelScene()=> SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        public void LevelUp()
        {
            _currentIndex++;
            PlayerPrefs.SetInt(GameConstants.PlayerPrefsLevel, _currentIndex);
            PlayerPrefs.Save();
        }
    }
}