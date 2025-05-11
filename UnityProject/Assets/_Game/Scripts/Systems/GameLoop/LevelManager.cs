using System.Collections.Generic;
using _Game.Core.Constants;
using UnityEngine;

namespace _Game.Systems.GameLoop
{
    public class LevelManager : MonoBehaviour
    {
        [Tooltip("Assign in build order.")]
        [SerializeField] private List<LevelData> allLevels;
        private int _currentIndex;

        public LevelData CurrentLevel =>
            (_currentIndex >= 0 && _currentIndex < allLevels.Count)
                ? allLevels[_currentIndex]
                : null;

        void Awake()
        {
            _currentIndex = PlayerPrefs.GetInt(GameConstants.PlayerPrefsLevel, 0);
            _currentIndex = Mathf.Clamp(_currentIndex, 0, allLevels.Count - 1);
            DontDestroyOnLoad(this);
        }

        public bool LoadNext()
        {
            if (_currentIndex + 1 >= allLevels.Count) return false;
            _currentIndex++;
            PlayerPrefs.SetInt(GameConstants.PlayerPrefsLevel, _currentIndex);
            PlayerPrefs.Save();
            // reload the scene or tell Bootstrapper to re‐run
            return true;
        }

        public void Reload() => LoadLevel(_currentIndex);

        public bool LoadLevel(int index)
        {
            if (index < 0 || index >= allLevels.Count) return false;
            _currentIndex = index;
            PlayerPrefs.SetInt(GameConstants.PlayerPrefsLevel, _currentIndex);
            PlayerPrefs.Save();
            return true;
        }
    }
}