#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.Rendering;
public class ChipsetDataLoader : EditorWindow
{
    public static string outputFolder = "Assets/ScriptableObjects/Chipset";
    private static string iconPath = "Assets/Materials/Icon/";
    public static string jsonFilePath { get; set; }
    public static bool createDatabase { get; set; }

    public static string chipsetComponentDatabasePath = $"{ChipsetComponentDataLoader.outputFolder}/ChipsetComponentDatabase.asset";

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
            List<ChipsetData> chipsetDataList = JsonConvert.DeserializeObject<List<ChipsetData>>(jsonText);
            List<ChipsetSO> createdChipsets = new List<ChipsetSO>();

            foreach (var chipsetData in chipsetDataList)
            {
                ChipsetSO chipsetSO = ScriptableObject.CreateInstance<ChipsetSO>();

                //데이터 복사
                chipsetSO.chipsetKey = chipsetData.chipsetKey;
                chipsetSO.chipsetName = chipsetData.name;
                chipsetSO.description = chipsetData.description;

                //칩셋 컴포넌트 추가
                if (!string.IsNullOrEmpty(chipsetData.chipsetComponentIDs))
                {
                    string[] temps = chipsetData.chipsetComponentIDs.Trim().Split(",");
                    int[] results = new int[temps.Length];
                    for (int i = 0; i < temps.Length; i++)
                    {
                        if (int.TryParse(temps[i], out int result))
                            results[i] = result;
                    }
                    chipsetSO.chipsetComponentIDs = results;
                }

                if (!string.IsNullOrEmpty(chipsetData.iconPath))
                {
                    chipsetSO.iconSprite = AssetDatabase.LoadAssetAtPath<Sprite>(iconPath + chipsetData.iconPath);

                    if (chipsetSO.iconSprite == null)
                        LogUtil.LogWarning($"{chipsetData.name}이름의 {chipsetData.iconPath}위치에 아이콘이 존재하지 않음");
                }

                //SO 저장
                string assetPath = $"{outputFolder}/Chipset_{chipsetData.name}.asset";
                AssetDatabase.CreateAsset(chipsetSO, assetPath);

                //에셋 이름 저장
                chipsetSO.name = $"Chipset_{chipsetData.name}";
                createdChipsets.Add(chipsetSO);

                EditorUtility.SetDirty(chipsetSO);
            }

            //데이터 베이스 생성
            if (createDatabase && createdChipsets.Count > 0)
            {
                ChipsetDatabaseSO database = ScriptableObject.CreateInstance<ChipsetDatabaseSO>();
                database.chipsets = createdChipsets;

                //칩셋 컴포넌트 데이터 베이스 값 할당
                database.chipsetComponentDatabase = AssetDatabase.LoadAssetAtPath<ChipsetComponentDatabaseSO>(chipsetComponentDatabasePath);
                if (database.chipsetComponentDatabase == null)
                    LogUtil.LogError("chipsetComponent Database 할당에 실패했습니다.");

                AssetDatabase.CreateAsset(database, $"{outputFolder}/ChipsetDatabase.asset");
                EditorUtility.SetDirty(database);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Sucess", $"Created {createdChipsets.Count} scriptable objects!", "OK");
        }
        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("Error", $"Failed to Convert JSON : {e.Message}", "OK");
            LogUtil.LogError($"JSON 변환 오류: {e}");
        }
    }
}
#endif
