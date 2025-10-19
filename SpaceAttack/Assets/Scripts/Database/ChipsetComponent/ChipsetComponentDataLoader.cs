#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Newtonsoft.Json;

public class ChipsetComponentDataLoader : EditorWindow
{
    public static string outputFolder = "Assets/ScriptableObjects/skillAndWeapon";
    private static string iconPath = "Assets/Materials/Icon/";
    public static string jsonFilePath { get; set; }
    public static bool createDatabase { get; set; }

    public static string chipsetDatabasePath = $"{ChipsetDataLoader.outputFolder}/ChipsetDatabase.asset";

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
            List<ChipsetComponentData> chipComponentDataList = JsonConvert.DeserializeObject<List<ChipsetComponentData>>(jsonText);

            List<ChipsetComponentSO> createdchipComponents = new List<ChipsetComponentSO>();

            foreach (var componentData in chipComponentDataList)
            {
                ChipsetComponentSO chipComponentSO = ScriptableObject.CreateInstance<ChipsetComponentSO>();

                //데이터 복사
                chipComponentSO.chipsetCompID = componentData.chipsetCompID;
                chipComponentSO.chipsetCpname = componentData.name;
                chipComponentSO.description = componentData.description;

                chipComponentSO.damageRate = GetFloatArray(componentData.damageRate);
                chipComponentSO.coolTime = GetFloatArray(componentData.coolTime);
                chipComponentSO.addedCritRate = GetFloatArray(componentData.addedCritRate);
                chipComponentSO.addedCritChanceRate = GetFloatArray(componentData.addedCritChanceRate);
                chipComponentSO.attackTime = GetFloatArray(componentData.attackTime);
                chipComponentSO.attackRange = GetFloatArray(componentData.attackRange);

                if (System.Enum.TryParse(componentData.componentTypeString.ToUpper(), out ChipsetComponentType parsedType))
                {
                    chipComponentSO.componentType = parsedType;
                }
                else
                {
                    LogUtil.LogError($"{componentData.name}의 타입이 {componentData.componentTypeString}으로 다르게 입력되었습니다.");
                }

                //아이콘 스프라이트 저장
                if (!string.IsNullOrEmpty(componentData.iconPath))
                {
                    chipComponentSO.iconSprite = AssetDatabase.LoadAssetAtPath<Sprite>(iconPath + componentData.iconPath);

                    if (chipComponentSO.iconSprite == null)
                        LogUtil.LogWarning($"{componentData.name}이름의 {componentData.iconPath}위치에 아이콘이 존재하지 않음");
                }

                //SO 저장
                string assetPath = $"{outputFolder}/ChipComp_{componentData.name}.asset";
                AssetDatabase.CreateAsset(chipComponentSO, assetPath);

                //에셋 이름 저장
                chipComponentSO.name = $"ChipsetComponent_{componentData.name}";
                createdchipComponents.Add(chipComponentSO);

                EditorUtility.SetDirty(chipComponentSO);
            }

            //데이터 베이서 생성
            if (createDatabase && createdchipComponents.Count > 0)
            {
                ChipsetComponentDatabaseSO database = ScriptableObject.CreateInstance<ChipsetComponentDatabaseSO>();
                database.chipsetComponents = createdchipComponents;

                //칩셋 컴포넌트 데이터 베이스 값 할당
                ChipsetDatabaseSO chipsetDatabase = AssetDatabase.LoadAssetAtPath<ChipsetDatabaseSO>(chipsetDatabasePath);
                chipsetDatabase.chipsetComponentDatabase = database;
                EditorUtility.SetDirty(chipsetDatabase);
                if (chipsetDatabase.chipsetComponentDatabase == null)
                    LogUtil.LogError("chipsetComponentDatabase 할당에 실패했습니다.");

                AssetDatabase.CreateAsset(database, $"{outputFolder}/ChipsetComponentDatabase.asset");
                EditorUtility.SetDirty(database);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Sucess", $"Created {createdchipComponents.Count} scriptable objects!", "OK");
        }
        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("Error", $"Failed to Convert JSON : {e.Message}", "OK");
            LogUtil.LogError($"JSON 변환 오류: {e}");
        }
    }

    private static float[] GetFloatArray(string text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            string[] temps = text.Trim().Split(",");
            float[] results = new float[temps.Length];
            for (int i = 0; i < temps.Length; i++)
            {
                if (float.TryParse(temps[i], out float result))
                    results[i] = result;
            }
            return results;
        }
        return null;
    }
}
#endif
