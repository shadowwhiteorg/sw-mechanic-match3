using _Game.Enums;
using UnityEditor;
using UnityEngine;

namespace _Game.Systems.GameLoop
{
    public class LevelEditorWindow : EditorWindow
{
    private LevelData _level;

    [MenuItem("Blast Tools/Level Editor")]
    public static void Open() => GetWindow<LevelEditorWindow>("Level Editor");

    private void OnGUI()
    {
        EditorGUILayout.Space();
        _level = (LevelData)EditorGUILayout.ObjectField("Level Data", _level, typeof(LevelData), false);
        if (_level == null) return;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Grid Settings", EditorStyles.boldLabel);
        _level.Rows    = EditorGUILayout.IntField("Rows",    _level.Rows);
        _level.Columns = EditorGUILayout.IntField("Columns", _level.Columns);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Gameplay", EditorStyles.boldLabel);
        _level.MoveLimit = EditorGUILayout.IntField("Move Limit", _level.MoveLimit);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Color Goals", EditorStyles.boldLabel);
        for (int i = 0; i < _level.ColorGoals.Count; i++)
        {
            var cg = _level.ColorGoals[i];
            EditorGUILayout.BeginHorizontal();
            cg.Color = (BlockColor)EditorGUILayout.EnumPopup(cg.Color);
            cg.Count = EditorGUILayout.IntField(cg.Count);
            if (GUILayout.Button("–", GUILayout.Width(20)))
                _level.ColorGoals.RemoveAt(i);
            _level.ColorGoals[i] = cg;
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("+ Add Color Goal"))
            _level.ColorGoals.Add(new LevelData.ColorGoal());

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Type Goals", EditorStyles.boldLabel);
        for (int i = 0; i < _level.TypeGoals.Count; i++)
        {
            var tg = _level.TypeGoals[i];
            EditorGUILayout.BeginHorizontal();
            tg.Type  = (BlockType)EditorGUILayout.EnumPopup(tg.Type);
            tg.Count = EditorGUILayout.IntField(tg.Count);
            if (GUILayout.Button("–", GUILayout.Width(20)))
                _level.TypeGoals.RemoveAt(i);
            _level.TypeGoals[i] = tg;
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("+ Add Type Goal"))
            _level.TypeGoals.Add(new LevelData.TypeGoal());

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Initial Blocks", EditorStyles.boldLabel);
        int total = _level.Rows * _level.Columns;
        _level.OnValidateTrigger(); // ensure correct length
        for (int i = 0; i < total; i++)
        {
            if (i % _level.Columns == 0) EditorGUILayout.BeginHorizontal();
            var bd = _level.InitialBlocks[i];
            bd.Color = (BlockColor)EditorGUILayout.EnumPopup(bd.Color, GUILayout.Width(60));
            bd.Type  = (BlockType) EditorGUILayout.EnumPopup(bd.Type,  GUILayout.Width(60));
            _level.InitialBlocks[i] = bd;
            if (i % _level.Columns == _level.Columns - 1) EditorGUILayout.EndHorizontal();
        }

        if (GUI.changed)
            EditorUtility.SetDirty(_level);
    }
}
}