#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using Core.Enums;
using UnityEditor;
using UnityEngine;

namespace Core.Data.Editor
{
    [CustomEditor(typeof(MapData))]
    public class MapDataEditor : UnityEditor.Editor
    {
        private int _rows = 7;
        private int _columns = 7;
        private List<GridCellData> _cells = new();
        private Vector2 _scrollPos;
        private string _exportFileName = "MapJson";

        private static readonly Color ColorNone       = new(0.75f, 0.75f, 0.75f);
        private static readonly Color ColorStrawberry  = new(1f, 0.45f, 0.45f);
        private static readonly Color ColorApple       = new(0.5f, 0.85f, 0.5f);
        private static readonly Color ColorPear        = new(1f, 0.92f, 0.35f);

        private const float CellWidth = 95f;
        private const float RowLabelWidth = 28f;
        private const float CellHeight = 46f;

        private void OnEnable()
        {
            LoadFromCurrentJson();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space(16);
            DrawSeparator();
            EditorGUILayout.LabelField("Map Editor", EditorStyles.boldLabel);
            EditorGUILayout.Space(4);

            DrawGridSizeControls();
            DrawActionButtons();

            EditorGUILayout.Space(8);

            if (_cells.Count > 0)
                DrawGridTable();
            else
                EditorGUILayout.HelpBox(
                    "Generate a grid or load from existing JSON to start editing.",
                    MessageType.Info);

            DrawExportSection();
        }

        private void DrawGridSizeControls()
        {
            EditorGUILayout.BeginHorizontal();
            _rows    = Mathf.Clamp(EditorGUILayout.IntField("Rows", _rows), 1, 20);
            _columns = Mathf.Clamp(EditorGUILayout.IntField("Columns", _columns), 1, 20);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawActionButtons()
        {
            EditorGUILayout.Space(4);
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Generate Empty Grid", GUILayout.Height(26)))
                GenerateEmptyGrid();

            if (GUILayout.Button("Load from JSON", GUILayout.Height(26)))
                LoadFromCurrentJson();

            if (_cells.Count > 0 && GUILayout.Button("Clear All Tiles", GUILayout.Height(26)))
                ClearAllTiles();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawGridTable()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(RowLabelWidth + 4);
            for (int x = 0; x < _columns; x++)
                EditorGUILayout.LabelField($"x:{x}", EditorStyles.centeredGreyMiniLabel,
                    GUILayout.Width(CellWidth));
            EditorGUILayout.EndHorizontal();

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.MaxHeight(520));

            for (int y = 0; y < _rows; y++)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField($"y:{y}", EditorStyles.centeredGreyMiniLabel,
                    GUILayout.Width(RowLabelWidth));

                for (int x = 0; x < _columns; x++)
                {
                    var cell = FindCell(x, y);
                    if (cell == null) continue;

                    var prevBg = GUI.backgroundColor;
                    GUI.backgroundColor = GetTileColor(cell.tileType);

                    EditorGUILayout.BeginVertical("box",
                        GUILayout.Width(CellWidth), GUILayout.Height(CellHeight));

                    cell.tileType = (TileType)EditorGUILayout.EnumPopup(cell.tileType,
                        GUILayout.Width(CellWidth - 12));

                    if (cell.tileType != TileType.None)
                    {
                        cell.count = Mathf.Max(0,
                            EditorGUILayout.IntField(cell.count,
                                GUILayout.Width(CellWidth - 12)));
                    }
                    else
                    {
                        cell.count = 0;
                        var prev = GUI.enabled;
                        GUI.enabled = false;
                        EditorGUILayout.IntField(0, GUILayout.Width(CellWidth - 12));
                        GUI.enabled = prev;
                    }

                    EditorGUILayout.EndVertical();
                    GUI.backgroundColor = prevBg;
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawExportSection()
        {
            EditorGUILayout.Space(12);
            DrawSeparator();
            EditorGUILayout.LabelField("Export", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("File Name", GUILayout.Width(65));
            _exportFileName = EditorGUILayout.TextField(_exportFileName);
            EditorGUILayout.LabelField(".json", GUILayout.Width(36));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(4);

            var prevColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.4f, 0.8f, 1f);

            if (GUILayout.Button("Export to JSON", GUILayout.Height(32)))
                ExportToJson();

            GUI.backgroundColor = prevColor;
        }

        private void GenerateEmptyGrid()
        {
            _cells.Clear();
            for (int y = 0; y < _rows; y++)
                for (int x = 0; x < _columns; x++)
                    _cells.Add(new GridCellData
                    {
                        x = x, y = y,
                        tileType = TileType.None,
                        count = 0
                    });
        }

        private void ClearAllTiles()
        {
            foreach (var cell in _cells)
            {
                cell.tileType = TileType.None;
                cell.count = 0;
            }
        }

        private void LoadFromCurrentJson()
        {
            var mapData = (MapData)target;
            if (!mapData.GridJson) return;

            var parsed = JsonUtility.FromJson<MapData.GridMapData>(mapData.GridJson.text);
            if (parsed == null) return;

            _rows    = parsed.rows;
            _columns = parsed.columns;
            _cells   = parsed.cells ?? new List<GridCellData>();

            _exportFileName = mapData.GridJson.name;
        }

        private void ExportToJson()
        {
            if (_cells.Count == 0)
            {
                EditorUtility.DisplayDialog("Export Failed",
                    "No grid data to export. Generate a grid first.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(_exportFileName))
            {
                EditorUtility.DisplayDialog("Export Failed",
                    "Please enter a file name.", "OK");
                return;
            }

            var gridData = new MapData.GridMapData
            {
                rows    = _rows,
                columns = _columns,
                cells   = new List<GridCellData>(_cells)
            };

            var json = JsonUtility.ToJson(gridData, true);

            const string directory = "Assets/_Game/Datas/LevelData";
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var filePath = $"{directory}/{_exportFileName}.json";
            File.WriteAllText(filePath, json);
            AssetDatabase.Refresh();

            var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(filePath);
            if (textAsset)
            {
                var so = new SerializedObject(target);
                var prop = so.FindProperty("GridJson");
                prop.objectReferenceValue = textAsset;
                so.ApplyModifiedProperties();
            }

            Debug.Log($"[MapDataEditor] JSON exported -> {filePath}");
        }

        private GridCellData FindCell(int x, int y)
        {
            foreach (var cell in _cells)
                if (cell.x == x && cell.y == y)
                    return cell;
            return null;
        }

        private static Color GetTileColor(TileType type) => type switch
        {
            TileType.Strawberry => ColorStrawberry,
            TileType.Apple      => ColorApple,
            TileType.Pear       => ColorPear,
            _                   => ColorNone
        };

        private static void DrawSeparator()
        {
            var rect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.5f));
            EditorGUILayout.Space(4);
        }
    }
}
#endif
