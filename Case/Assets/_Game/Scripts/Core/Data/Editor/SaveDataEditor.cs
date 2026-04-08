#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Core.Data.Editor
{
    [CustomEditor(typeof(SaveData))]
    public class SaveDataEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUILayout.Space(12);
            EditorGUILayout.LabelField("Save Tools", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Clears only the PlayerPrefs keys defined above. Other game data is untouched.", MessageType.Info);

            GUILayout.Space(4);

            var prevColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(1f, 0.35f, 0.35f);

            if (GUILayout.Button("🗑  Clear Save Data", GUILayout.Height(32)))
            {
                bool confirmed = EditorUtility.DisplayDialog(
                    title:   "Clear Save Data",
                    message: "This will permanently delete all saved inventory counts from PlayerPrefs.\n\nContinue?",
                    ok:      "Yes, Clear All",
                    cancel:  "Cancel");

                if (confirmed)
                {
                    var saveData = (SaveData)target;
                    saveData.ClearAllSaveData();

                    EditorUtility.SetDirty(target);
                    Debug.Log("[SaveDataEditor] Save data cleared via Inspector button.");
                }
            }

            GUI.backgroundColor = prevColor;
        }
    }
}
#endif

