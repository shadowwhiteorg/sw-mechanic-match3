using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace _Game.Utils.Editor.Diagnostics.MissingScript
{
    public class MissingScriptFinderWindow : EditorWindow
    {
        private List<ScanResult> results = new();
        private Vector2 scrollPos;
        private bool searchInPrefabs = true;
        private bool searchInScenes = true;
        private string folderFilter = "Assets";
        private List<SceneAsset> scenesToScan = new();

        [MenuItem("Tools/Diagnostics/Missing Script Finder")]
        public static void Open()
        {
            GetWindow<MissingScriptFinderWindow>("Missing Scripts");
        }

        private void OnGUI()
        {
            GUILayout.Label("Missing Script Scanner", EditorStyles.boldLabel);
            searchInScenes = GUILayout.Toggle(searchInScenes, "Include Open Scene");
            searchInPrefabs = GUILayout.Toggle(searchInPrefabs, "Include Project Prefabs");

            EditorGUILayout.Space();
            folderFilter = EditorGUILayout.TextField("Folder Filter (for Prefabs)", folderFilter);

            EditorGUILayout.LabelField("Batch Scene Scan:");
            int sceneCount = Mathf.Max(0, EditorGUILayout.IntField("Scene Count", scenesToScan.Count));
            while (scenesToScan.Count < sceneCount)
                scenesToScan.Add(null);
            while (scenesToScan.Count > sceneCount)
                scenesToScan.RemoveAt(scenesToScan.Count - 1);

            for (int i = 0; i < scenesToScan.Count; i++)
            {
                scenesToScan[i] = (SceneAsset)EditorGUILayout.ObjectField($"Scene {i + 1}", scenesToScan[i], typeof(SceneAsset), false);
            }

            if (GUILayout.Button("Scan"))
            {
                results.Clear();
                if (searchInScenes)
                    results.AddRange(MissingScriptScanner.ScanScene());
                if (searchInPrefabs)
                    results.AddRange(MissingScriptScanner.ScanPrefabs(folderFilter));
                foreach (var sceneAsset in scenesToScan)
                {
                    if (sceneAsset != null)
                        results.AddRange(MissingScriptScanner.ScanSceneAsset(sceneAsset));
                }
            }

            if (GUILayout.Button("Clean All"))
            {
                foreach (var result in results)
                    MissingScriptScanner.CleanMissingComponents(result.GameObject);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log($"[MissingScriptFinder] Cleaned {results.Count} entries.");
                results.Clear();
            }

            GUILayout.Space(10);
            if (GUILayout.Button("Export Results to CSV"))
                ExportResultsToCSV();

            if (GUILayout.Button("Export Results to JSON"))
                ExportResultsToJSON();

            EditorGUILayout.Space();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            foreach (var result in results)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(result.GameObject, typeof(GameObject), true);
                EditorGUILayout.LabelField(result.Summary, EditorStyles.wordWrappedLabel);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }

        private void ExportResultsToCSV()
        {
            string path = EditorUtility.SaveFilePanel("Export Missing Script Report to CSV", "", "MissingScriptReport.csv", "csv");
            if (string.IsNullOrEmpty(path)) return;

            using var writer = new StreamWriter(path);
            writer.WriteLine("AssetPath,GameObjectPath,MissingIndex");
            foreach (var r in results)
                writer.WriteLine($"{r.AssetPath},{r.GameObjectPath},{r.MissingIndex}");
            Debug.Log($"Exported to CSV: {path}");
        }

        private void ExportResultsToJSON()
        {
            string path = EditorUtility.SaveFilePanel("Export Missing Script Report to JSON", "", "MissingScriptReport.json", "json");
            if (string.IsNullOrEmpty(path)) return;

            var exportList = new List<SerializableScanResult>();
            foreach (var r in results)
            {
                exportList.Add(new SerializableScanResult
                {
                    GameObjectPath = r.GameObjectPath,
                    AssetPath = r.AssetPath,
                    MissingIndex = r.MissingIndex
                });
            }

            string json = JsonUtility.ToJson(new ScanResultWrapper { Items = exportList.ToArray() }, true);
            File.WriteAllText(path, json);
            Debug.Log($"Exported to JSON: {path}");
        }

        [Serializable]
        public class SerializableScanResult
        {
            public string GameObjectPath;
            public string AssetPath;
            public int MissingIndex;
        }

        [Serializable]
        public class ScanResultWrapper
        {
            public SerializableScanResult[] Items;
        }

        [MenuItem("Tools/Diagnostics/CLI Scan Export JSON")]
        public static void CLIExportJson()
        {
            var allResults = new List<ScanResult>();
            allResults.AddRange(MissingScriptScanner.ScanScene());
            allResults.AddRange(MissingScriptScanner.ScanPrefabs());

            var exportList = new List<SerializableScanResult>();
            foreach (var r in allResults)
            {
                exportList.Add(new SerializableScanResult
                {
                    GameObjectPath = r.GameObjectPath,
                    AssetPath = r.AssetPath,
                    MissingIndex = r.MissingIndex
                });
            }

            string outputPath = "Assets/Editor/Diagnostics/MissingScriptReport_CLI.json";
            string json = JsonUtility.ToJson(new ScanResultWrapper { Items = exportList.ToArray() }, true);
            File.WriteAllText(outputPath, json);
            Debug.Log($"CLI Export complete to: {outputPath}");
        }
    }
}
