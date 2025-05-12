using UnityEditor;
using UnityEngine;

namespace _Game.Core.Data
{
    [InitializeOnLoad]
    public static class LevelLibrarySync
    {
        static LevelLibrarySync()
        {
            // Delay to let Unity finish domain reload
            EditorApplication.delayCall += SyncLibrary;
        }

        private static void SyncLibrary()
        {
            var libPath = "Assets/Levels/LevelLibrary.asset";
            var lib     = AssetDatabase.LoadAssetAtPath<LevelLibrary>(libPath);
            if (lib == null) return;

            lib.levelJsonAssets.Clear();

            // find all .json TextAssets in Assets/Levels/
            var guids = AssetDatabase.FindAssets("t:TextAsset", new[] { "Assets/Levels" });
            foreach (var g in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(g);
                if (path.EndsWith(".json"))
                {
                    var ta = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                    if (ta != null)
                        lib.levelJsonAssets.Add(ta);
                }
            }

            EditorUtility.SetDirty(lib);
            AssetDatabase.SaveAssets();
            Debug.Log("[LevelLibrarySync] Synced LevelLibrary with JSON files.");
        }
    }

}