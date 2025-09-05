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
                chipComponentSO.chipsetComponentKey = componentData.chipsetComponentKey;
                chipComponentSO.name = componentData.name;
                chipComponentSO.description = componentData.description;

                if (System.Enum.TryParse(componentData.componentTypeString, out ChipsetComponentType parsedType))
                {
                    chipComponentSO.componentType = parsedType;
                }
                else
                {
                    Debug.LogError($"{componentData.name}의 타입이 {componentData.componentTypeString}으로 다르게 입력되었습니다.");
                }

                //아이콘 스프라이트 저장
                if (!string.IsNullOrEmpty(componentData.iconPath))
                {
                    chipComponentSO.iconSprite = AssetDatabase.LoadAssetAtPath<Sprite>(iconPath + componentData.iconPath);

                    if (chipComponentSO.iconSprite == null)
                        Debug.LogWarning($"{componentData.name}이름의 {componentData.iconPath}위치에 아이콘이 존재하지 않음");
                }

                //SO 저장
                string assetPath = $"{outputFolder}/{componentData.name}_Icon.asset";
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
            Debug.LogError($"JSON 변환 오류: {e}");
        }
    }
}
#endif
