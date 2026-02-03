#if UNITY_EDITOR
using System.IO;
using System.Reflection;
using TeamSuneat;
using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using UnityEditor;
using UnityEngine;

namespace TeamSuneat.Development
{
    /// <summary>
    /// Development Tools Window
    /// All text displayed in the development tool window is shown in English.
    /// </summary>
    public class DevelopmentWindow : EditorWindow
    {
        private DevelopmentToolsGUI _gui;
        private const int DEFAULT_GAME_DATA_COUNT = 3;

        [MenuItem("Tools/Development Tools Window")]
        private static void ShowWindow()
        {
            DevelopmentWindow window = GetWindow<DevelopmentWindow>("Development Tools");
            window.Show();
        }

        private void OnEnable()
        {
            InitializeGUI();
        }

        private void InitializeGUI()
        {
            _gui = new DevelopmentToolsGUI();
            // OnEnable에서는 EditorStyles가 null일 수 있으므로 OnGUI에서 초기화
        }

        private void OnGUI()
        {
            // OnGUI 내부에서는 EditorStyles가 유효하므로 스타일 초기화 확인
            if (_gui.TitleStyle == null)
            {
                _gui.RefreshStyle(isEditor: true);
            }

            _gui.ScrollPosition = EditorGUILayout.BeginScrollView(_gui.ScrollPosition);
            DrawDevelopmentToolsTab();
            EditorGUILayout.EndScrollView();
        }

        //

        private void DrawDevelopmentToolsTab()
        {
            _gui.DrawTitleLabel("[Development Tools Window]");
            EditorGUILayout.Space(10);

            DrawPathManagerSection();
            EditorGUILayout.Space(10);

            DrawJsonDataManagerSection();
            EditorGUILayout.Space(10);

            DrawExcelSection();
            EditorGUILayout.Space(10);

            DrawGoogleSheetsSection();
            EditorGUILayout.Space(10);

            DrawGameDataManagerSection();
            EditorGUILayout.Space(10);

            DrawLogLevelSection();
            EditorGUILayout.Space(10);

            DrawGamePrefsSection();
        }

        //

        private void DrawPathManagerSection()
        {
            _ = EditorGUILayout.BeginVertical("box");
            _gui.DrawTitleLabel("Path Manager");

            if (GUILayout.Button("Save File Path", GUILayout.Width(250)))
            {
                PathManager.UpdatePathMetaData();
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawJsonDataManagerSection()
        {
            _ = EditorGUILayout.BeginVertical("box");
            _gui.DrawTitleLabel("Json Data Manager");

            if (GUILayout.Button("Load JSON Sheet", GUILayout.Width(250)))
            {
                JsonDataManager.LoadJsonSheetsSync();
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawExcelSection()
        {
            _ = EditorGUILayout.BeginVertical("box");
            _gui.DrawTitleLabel("Excel");

            if (GUILayout.Button("Load All Excel Files", GUILayout.Width(250)))
            {
                Excel4Unity.ConvertAllExcelToJSON();
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawGoogleSheetsSection()
        {
            EditorGUILayout.BeginVertical("box");
            {
                _gui.DrawTitleLabel("Google Sheets");

                if (GUILayout.Button("Load All Sheets (GID List)", GUILayout.Width(250)))
                {
                    GoogleSheetsMenu.LoadMultipleSheetsByGIDs();
                }
                if (GUILayout.Button("Convert All to JSON", GUILayout.Width(250)))
                {
                    GoogleSheetsMenu.ConvertAllToJson();
                }
                if (GUILayout.Button("Convert Stat to JSON", GUILayout.Width(250)))
                {
                    GoogleSheetsMenu.ConvertStatToJson();
                }
                if (GUILayout.Button("Convert String to JSON", GUILayout.Width(250)))
                {
                    GoogleSheetsMenu.ConvertStringToJson();
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawGameDataManagerSection()
        {
            _ = EditorGUILayout.BeginVertical("box");
            _gui.DrawTitleLabel("Game Data Manager");

            if (GUILayout.Button("Delete Save Data", GUILayout.Width(250)))
            {
                EditorApplication.delayCall += () =>
                {
                    string fileName = $"{Application.productName}.json";
                    bool shouldDelete = EditorUtility.DisplayDialog(
                        "Delete Save Data",
                        $"Delete Release Save Data.\n\n- Target: \"{fileName}\" file in persistentDataPath\n- Development Save Data (\"_Dev.json\") is not deleted.\n\nAre you sure you want to delete?",
                        "Delete",
                        "Cancel");

                    if (shouldDelete)
                    {
                        DeleteReleaseSaveFilesForEditor();
                    }
                };
            }

            if (GUILayout.Button("Delete Development Save Data", GUILayout.Width(250)))
            {
                EditorApplication.delayCall += () =>
                {
                    string fileName = $"{Application.productName}_Dev.json";
                    bool shouldDelete = EditorUtility.DisplayDialog(
                        "Delete Development Save Data",
                        $"Delete Development Save Data.\n\n- Target: \"{fileName}\" file in persistentDataPath\n\nAre you sure you want to delete?",
                        "Delete",
                        "Cancel");

                    if (shouldDelete)
                    {
                        GameDataManager.DeleteSaveFileForEditor();
                    }
                };
            }

            EditorGUILayout.EndVertical();
        }

        private void DeleteReleaseSaveFilesForEditor()
        {
            int gameDataCount = GetGameDataCountForEditor();
            string saveDirectory = Application.persistentDataPath;
            string productName = Application.productName;

            for (int i = 0; i < gameDataCount; i++)
            {
                string fileName = $"{productName}{i + 1}.json";
                string saveFilePath = Path.Combine(saveDirectory, fileName);

                if (File.Exists(saveFilePath))
                {
                    File.Delete(saveFilePath);
                    Debug.Log($"Delete Local Save File. SaveFilePath: {saveFilePath}");
                }
                else
                {
                    Debug.Log($"Local Save File is already deleted. SaveFilePath: {saveFilePath}");
                }
            }
        }

        private int GetGameDataCountForEditor()
        {
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Static;
            FieldInfo fieldInfo = typeof(GameDataManager).GetField("GAME_DATA_COUNT", flags);
            if (fieldInfo == null || !fieldInfo.IsLiteral)
            {
                return DEFAULT_GAME_DATA_COUNT;
            }

            object rawValue = fieldInfo.GetRawConstantValue();
            return rawValue is int value ? value : DEFAULT_GAME_DATA_COUNT;
        }

        private void DrawLogLevelSection()
        {
            _ = EditorGUILayout.BeginVertical("box");
            _gui.DrawTitleLabel("Log Level");

            bool progress = Log.LevelProgress;
            bool info = Log.LevelInfo;
            bool warning = Log.LevelWarning;
            bool error = Log.LevelError;
            bool except = Log.LevelExcept;

            bool newProgress = EditorGUILayout.Toggle("Progress", progress);
            if (newProgress != progress)
            {
                Log.SwitchLogLevelProgress();
            }

            bool newInfo = EditorGUILayout.Toggle("Info", info);
            if (newInfo != info)
            {
                Log.SwitchLogLevelInfo();
            }

            bool newWarning = EditorGUILayout.Toggle("Warning", warning);
            if (newWarning != warning)
            {
                Log.SwitchLogLevelWarning();
            }

            bool newError = EditorGUILayout.Toggle("Error", error);
            if (newError != error)
            {
                Log.SwitchLogLevelError();
            }

            bool newExcept = EditorGUILayout.Toggle("Except", except);
            if (newExcept != except)
            {
                Log.SwitchLogLevelExcept();
            }

            EditorGUILayout.Space(5);

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("All On", GUILayout.Width(120)))
            {
                Log.SetLogLevelAll();
            }

            if (GUILayout.Button("All Off", GUILayout.Width(120)))
            {
                Log.SetLogLevelOff();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        private void DrawGamePrefsSection()
        {
            _ = EditorGUILayout.BeginVertical("box");
            _gui.DrawTitleLabel("Game Prefs");

            if (GUILayout.Button("모든 설정 삭제", GUILayout.Width(250)))
            {
                GamePrefs.DeleteAllSettings();
            }
            EditorGUILayout.EndVertical();
        }
    }
}
#endif