using System;
using System.Collections.Generic;
using _Game.Enums;
using UnityEditor;
using UnityEngine;

namespace _Game.Systems.GameLoop
{
    public class LevelEditorWindow : EditorWindow
    {
        private LevelData _level;
        private Vector2   _scrollPos;

        // Fixed dimensions for each grid cell
        private const float CellWidth  = 90f;
        private const float CellHeight = 80f;

        [MenuItem("Blast Tools/Level Editor")]
        public static void Open() => GetWindow<LevelEditorWindow>("Level Editor");

        private void OnGUI()
        {
            EditorGUILayout.Space();
            _level = (LevelData)EditorGUILayout.ObjectField(
                "Level Data",
                _level,
                typeof(LevelData),
                false
            );
            if (_level == null) return;

            DrawGridSettings();
            DrawGameplaySettings();
            DrawColorGoals();
            DrawTypeGoals();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Initial Blocks", EditorStyles.boldLabel);

            // Wrap the grid in a scroll view so large layouts can scroll
            _scrollPos = EditorGUILayout.BeginScrollView(
                _scrollPos,
                GUILayout.ExpandHeight(true)
            );
            DrawInitialBlocks();
            EditorGUILayout.EndScrollView();

            if (GUI.changed)
                EditorUtility.SetDirty(_level);
        }

        private void DrawGridSettings()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Grid Settings", EditorStyles.boldLabel);
            _level.Rows    = EditorGUILayout.IntField("Rows",    _level.Rows);
            _level.Columns = EditorGUILayout.IntField("Columns", _level.Columns);
        }

        private void DrawGameplaySettings()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Gameplay", EditorStyles.boldLabel);
            _level.MoveLimit = EditorGUILayout.IntField("Move Limit", _level.MoveLimit);
        }

        private void DrawColorGoals()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Color Goals", EditorStyles.boldLabel);
            for (int i = 0; i < _level.ColorGoals.Count; i++)
            {
                var cg = _level.ColorGoals[i];
                EditorGUILayout.BeginHorizontal();
                    cg.Color = ColorCodedEnumPopup(cg.Color, GUILayout.Width(80));
                    cg.Count = EditorGUILayout.IntField(cg.Count);
                    if (GUILayout.Button("–", GUILayout.Width(20)))
                        _level.ColorGoals.RemoveAt(i);
                    else
                        _level.ColorGoals[i] = cg;
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("+ Add Color Goal"))
                _level.ColorGoals.Add(new LevelData.ColorGoal());
        }

        private void DrawTypeGoals()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Type Goals", EditorStyles.boldLabel);
            for (int i = 0; i < _level.TypeGoals.Count; i++)
            {
                var tg = _level.TypeGoals[i];
                EditorGUILayout.BeginHorizontal();
                    tg.Type  = ColorCodedEnumPopup(tg.Type, GUILayout.Width(80));
                    tg.Count = EditorGUILayout.IntField(tg.Count);
                    if (GUILayout.Button("–", GUILayout.Width(20)))
                        _level.TypeGoals.RemoveAt(i);
                    else
                        _level.TypeGoals[i] = tg;
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("+ Add Type Goal"))
                _level.TypeGoals.Add(new LevelData.TypeGoal());
        }

        private void DrawInitialBlocks()
        {
            int rows    = _level.Rows;
            int columns = _level.Columns;
            _level.OnValidateTrigger(); // ensure InitialBlocks list length

            for (int r = 0; r < rows; r++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int c = 0; c < columns; c++)
                {
                    int index = r * columns + c;
                    var bd    = _level.InitialBlocks[index];

                    EditorGUILayout.BeginVertical(
                        "box",
                        GUILayout.Width( CellWidth ),
                        GUILayout.MinHeight(CellHeight),
                        GUILayout.MaxHeight(CellHeight),
                        GUILayout.ExpandHeight(false)
                    );

                    // 1) Block Type (always)
                    bd.Type = ColorCodedEnumPopup(
                        bd.Type,
                        GUILayout.Width(CellWidth - 10)
                    );

                    // 2) Color only for plain blocks
                    ConditionalField(
                        show: bd.Type == BlockType.None,
                        drawField: () =>
                        {
                            bd.Color = ColorCodedEnumPopup(
                                bd.Color,
                                GUILayout.Width(CellWidth - 10)
                            );
                        },
                        width: CellWidth - 10
                    );

                    // 3) Direction only for rockets
                    ConditionalField(
                        show: bd.Type == BlockType.Rocket,
                        drawField: () =>
                        {
                            bd.Direction = (BlockDirection)EditorGUILayout.EnumPopup(
                                bd.Direction,
                                GUILayout.Width(CellWidth - 10)
                            );
                        },
                        width: CellWidth - 10
                    );

                    _level.InitialBlocks[index] = bd;
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// Draws either the given field or a spacer of equal width to keep columns aligned.
        /// </summary>
        private void ConditionalField(bool show, Action drawField, float width)
        {
            if (show)
                drawField();
            else
                GUILayout.Space(width + 4); // +4 for padding/margin
        }

        /// <summary>
        /// Renders an EnumPopup with a background color mapped to the enum value.
        /// </summary>
        private T ColorCodedEnumPopup<T>(T value, params GUILayoutOption[] options) where T : Enum
        {
            var prevColor = GUI.backgroundColor;
            GUI.backgroundColor = GetColorFor(value);
            value = (T)EditorGUILayout.EnumPopup(value, options);
            GUI.backgroundColor = prevColor;
            return value;
        }

        /// <summary>
        /// Maps each BlockColor and BlockType to a distinct Color.
        /// </summary>
        private Color GetColorFor<T>(T enumVal) where T : Enum
        {
            if (enumVal is BlockColor color)
                return color switch
                {
                    BlockColor.Red    => Color.red,
                    BlockColor.Blue   => Color.blue,
                    BlockColor.Green  => Color.green,
                    BlockColor.Yellow => Color.yellow,
                    BlockColor.Purple => new Color(0.6f, 0, 0.6f),
                    _                 => Color.white
                };
            if (enumVal is BlockType type)
                return type switch
                {
                    BlockType.Rocket => Color.cyan,
                    BlockType.Bomb   => Color.grey,
                    BlockType.Duck   => Color.yellow,
                    BlockType.Balloon   => Color.magenta,
                    _                => Color.white
                };
            return Color.white;
        }
    }
}
