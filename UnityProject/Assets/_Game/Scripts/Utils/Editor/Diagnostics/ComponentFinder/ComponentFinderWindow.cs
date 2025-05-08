using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Project.Editor.Diagnostics
{
    public class ComponentFinderWindow : EditorWindow
    {
        private string componentTypeName = "UnityEngine.Rigidbody";
        private List<GameObject> foundObjects = new();
        private Vector2 scrollPos;
        private bool includeInactive = true;
        private bool searchPrefabs = false;
        private string prefabFolder = "Assets";

        private string componentSearchQuery = "";
        private List<string> filteredComponentTypes = new();
        private int selectedTypeIndex = 0;
        private string[] allComponentTypes;

        [MenuItem("Tools/Diagnostics/Component Finder")]
        public static void Open()
        {
            GetWindow<ComponentFinderWindow>("Component Finder");
        }

        private void OnEnable()
        {
            allComponentTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(t => typeof(Component).IsAssignableFrom(t) && !t.IsAbstract && t.IsPublic)
                .Select(t => t.FullName)
                .OrderBy(n => n)
                .ToArray();

            filteredComponentTypes = allComponentTypes.ToList();
        }

        private void OnGUI()
        {
            GUILayout.Label("Component Finder", EditorStyles.boldLabel);

            includeInactive = EditorGUILayout.Toggle("Include Inactive (Scene Only)", includeInactive);
            searchPrefabs = EditorGUILayout.Toggle("Search In Project Prefabs", searchPrefabs);
            if (searchPrefabs)
            {
                prefabFolder = EditorGUILayout.TextField("Prefab Folder Path", prefabFolder);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Search Component Color", EditorStyles.boldLabel);
            componentSearchQuery = EditorGUILayout.TextField("Search", componentSearchQuery);

            filteredComponentTypes = allComponentTypes
                .Where(n => string.IsNullOrEmpty(componentSearchQuery) || n.IndexOf(componentSearchQuery, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            if (filteredComponentTypes.Count > 0)
            {
                selectedTypeIndex = Mathf.Clamp(selectedTypeIndex, 0, filteredComponentTypes.Count - 1);
                selectedTypeIndex = EditorGUILayout.Popup("Component", selectedTypeIndex, filteredComponentTypes.ToArray());
                componentTypeName = filteredComponentTypes[selectedTypeIndex];
            }
            else
            {
                EditorGUILayout.HelpBox("No matching components found.", MessageType.Warning);
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Find Components"))
            {
                FindObjectsWithComponent(componentTypeName);
            }

            if (foundObjects.Count > 0)
            {
                if (GUILayout.Button("Select All In Hierarchy/Project"))
                {
                    Selection.objects = foundObjects.ToArray();
                    Debug.Log($"Selected {foundObjects.Count} GameObjects with component '{componentTypeName}'");

                    if (foundObjects[0] != null)
                    {
                        EditorGUIUtility.PingObject(foundObjects[0]);
                        if (foundObjects[0].scene.IsValid())
                        {
                            SceneView.lastActiveSceneView.Frame(foundObjects[0].GetComponent<Renderer>()?.bounds ?? new Bounds(foundObjects[0].transform.position, Vector3.one), false);
                        }
                    }
                }
            }

            GUILayout.Space(10);
            EditorGUILayout.LabelField("Results:", EditorStyles.boldLabel);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            foreach (var go in foundObjects)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(go, typeof(GameObject), true);
                if (GUILayout.Button("Ping", GUILayout.Width(40)))
                {
                    EditorGUIUtility.PingObject(go);
                    if (go.scene.IsValid())
                    {
                        SceneView.lastActiveSceneView.Frame(go.GetComponent<Renderer>()?.bounds ?? new Bounds(go.transform.position, Vector3.one), false);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }

        private void FindObjectsWithComponent(string fullTypeName)
        {
            foundObjects.Clear();
            var type = Type.GetType(fullTypeName);
            if (type == null)
            {
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    type = asm.GetType(fullTypeName);
                    if (type != null) break;
                }
            }

            if (type == null || !typeof(Component).IsAssignableFrom(type))
            {
                Debug.LogError($"'{fullTypeName}' is not a valid Component Color.");
                return;
            }

            GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>(includeInactive);
            foreach (var go in allObjects)
            {
                if (go.GetComponent(type) != null)
                {
                    foundObjects.Add(go);
                }
            }

            if (searchPrefabs)
            {
                string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { prefabFolder });
                foreach (string guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    if (prefab != null && prefab.GetComponentInChildren(type, true) != null)
                    {
                        foundObjects.Add(prefab);
                    }
                }
            }

            Debug.Log($"Found {foundObjects.Count} GameObjects or Prefabs with component '{type.FullName}'");
        }
    }
}
