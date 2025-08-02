#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class JsonToScriptableConverter : EditorWindow
{
    private string jsonFilePath = "";
    private bool createDatabase = true;  //데이터 베이스 생성 여부
    private JsonType jsonType = JsonType.Sound;           //Json타입

    //private System.Enum JsonType;

    [MenuItem("Tools/JSON to Scriptable Objects")]
    public static void ShowWindow()
    {
        GetWindow<JsonToScriptableConverter>("JSON to Scriptable Objects");
    }

    private void OnGUI()
    {
        GUILayout.Label("JSON to Scriptable Object Converter", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if (GUILayout.Button("Select JSON File"))
        {
            jsonFilePath = EditorUtility.OpenFilePanel("Select JSON File", "", "json");
        }

        EditorGUILayout.LabelField("Selected File : ", jsonFilePath);
        EditorGUILayout.Space();
        createDatabase = EditorGUILayout.Toggle("Create Database Asset", createDatabase);
        EditorGUILayout.Space();
        GUILayout.Label("Selected JsonType", EditorStyles.label);
        jsonType = (JsonType)EditorGUILayout.EnumPopup("Create JsonType", jsonType);

        if (GUILayout.Button("Convert to Scriptable Objects"))
        {
            if (string.IsNullOrEmpty(jsonFilePath))
            {
                EditorUtility.DisplayDialog("Error", "Please select a JSON file firest!", "OK");
                return;
            }
            SelectConverter();
        }
    }

    private void SelectConverter()
    {
        switch (jsonType)
        {
            case JsonType.Sound:
                SoundDataLoader.jsonFilePath = jsonFilePath;
                SoundDataLoader.createDatabase = createDatabase;
                SoundDataLoader.ConvertJsonToScriptableObjects();
            break;

            case JsonType.Player:
                Debug.Log("플레이어 제이슨 저장");
            break;

            case JsonType.Monster:
                Debug.Log("몬스터 제이슨 저장");
            break;
        }
    }
}
#endif
