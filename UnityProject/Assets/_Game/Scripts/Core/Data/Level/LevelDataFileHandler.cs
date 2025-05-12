// File: LevelDataFileHandler.cs
using System.IO;
using UnityEngine;
using _Game.Systems.GameLoop;

namespace _Game.Core.Data
{
    public static class LevelDataFileHandler
    {
        private const string LevelsFolder = "Assets/_Game/Data/ScriptableObjects/LevelData/JSON";
        private const string JsonExt = ".json";
        
        public static void Save(LevelData level, string fileName)
        {
            if (!Directory.Exists(LevelsFolder))
                Directory.CreateDirectory(LevelsFolder);

            var path = Path.Combine(LevelsFolder, fileName + JsonExt);
            var json = JsonUtility.ToJson(level, prettyPrint: true);
            File.WriteAllText(path, json);
            Debug.Log($"[LevelDataFileHandler] Saved: {path}");

            // Import into Unity so it becomes a TextAsset
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.ImportAsset(path);
            UnityEditor.AssetDatabase.Refresh();
#endif
        }
        
        public static LevelData Load(TextAsset jsonAsset)
        {
            if (jsonAsset == null)
            {
                Debug.LogError("[LevelDataFileHandler] Null TextAsset passed to Load");
                return null;
            }

            var clone = ScriptableObject.CreateInstance<LevelData>();
            JsonUtility.FromJsonOverwrite(jsonAsset.text, clone);
            return clone;
        }
    }
}