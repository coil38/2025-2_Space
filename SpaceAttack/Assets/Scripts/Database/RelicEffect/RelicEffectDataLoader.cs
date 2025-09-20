#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Newtonsoft.Json;
public class RelicEffectDataLoader : EditorWindow
{
    public static string outputFolder = "Assets/ScriptableObjects/RelicEffect";
    private static string iconPath = "Assets/Materials/Icon/";
    public static string jsonFilePath { get; set; }
    public static bool createDatabase { get; set; }

    public static string relicDatabasePath = $"{RelicDataLoader.outputFolder}/RelicDatabase.asset";

    public static void ConvertJsonToScriptableObjects()
    {
        if (!Directory.Exists(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
        }

        string jsonText = File.ReadAllText(jsonFilePath);

        try
        {
            //JSON 파싱
            List<RelicEffectData> relicEffectDataList = JsonConvert.DeserializeObject<List<RelicEffectData>>(jsonText);
            List<RelicEffectSO> createdRelicEffects = new List<RelicEffectSO>();

            foreach (var relicEffectData in relicEffectDataList)
            {
                RelicEffectSO relicEffectSO = ScriptableObject.CreateInstance<RelicEffectSO>();

                //데이터 복사
                relicEffectSO.relicEffectId = relicEffectData.relicEffectID;
                relicEffectSO.relicEffectName = relicEffectData.name;
                relicEffectSO.relicEffectDiscription = relicEffectData.description;

                //SO 저장
                string assetPath = $"{outputFolder}/RelicEffect_{relicEffectData.name}.asset";
                AssetDatabase.CreateAsset(relicEffectSO, assetPath);

                //에셋 이름 저장
                relicEffectSO.name = $"RelicEffect_{relicEffectData.name}";
                createdRelicEffects.Add(relicEffectSO);

                EditorUtility.SetDirty(relicEffectSO);
            }

            //데이터 베이스 생성
            if (createDatabase && createdRelicEffects.Count > 0)
            {
                RelicEffectDatabaseSO database = ScriptableObject.CreateInstance<RelicEffectDatabaseSO>();
                database.relicEffects = createdRelicEffects;

                //유물 효과 데이터 베이스 값 할당
                RelicDatabaseSO relicDatabase = AssetDatabase.LoadAssetAtPath<RelicDatabaseSO>(relicDatabasePath);
                relicDatabase.relicEffectDatabase = database;
                EditorUtility.SetDirty(relicDatabase);
                if (relicDatabase.relicEffectDatabase == null)
                    LogUtil.LogError("RelicEffectDatabase 할당에 실패했습니다.");

                AssetDatabase.CreateAsset(database, $"{outputFolder}/RelicEffectDatabase.asset");
                EditorUtility.SetDirty(database);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Sucess", $"Created {createdRelicEffects.Count} scriptable objects!", "OK");
        }
        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("Error", $"Failed to Convert JSON : {e.Message}", "OK");
            LogUtil.LogError($"JSON 변환 오류: {e}");
        }
    }
}
#endif
