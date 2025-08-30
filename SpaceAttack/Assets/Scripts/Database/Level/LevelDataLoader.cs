#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Newtonsoft.Json;

public class LevelDataLoader : EditorWindow
{
    private static string outputFolder = "Assets/ScriptableObjects/levels";
    
    public static string jsonFilePath { get; set; }
    public static bool creatDatabase { get; set; }

    public static void ConvertJsonToScriptableObjects()
    {
        //폴더 생성
        if (!Directory.Exists(outputFolder))   //폴더 위치를 폴더가 없을 경우, 생성
        {
            Directory.CreateDirectory(outputFolder);
        }

        //JSON 파일 읽기
        string jsonText = File.ReadAllText(jsonFilePath);

        try
        {
            //JSON 파싱
            List<LevelData> levelDataList = JsonConvert.DeserializeObject<List<LevelData>>(jsonText);

            List<LevelSO> createdLevels = new List<LevelSO>();  //levelSO 리스트

            foreach (var levelData in levelDataList)
            {
                LevelSO levelSO = ScriptableObject.CreateInstance<LevelSO>();

                //데이터 복사
                levelSO.levelKey = levelData.levelKey;
                levelSO.level = levelData.level;
                levelSO.maxEX = levelData.maxEX;
                levelSO.damageCorrection = levelData.damageCorrection.HasValue ? levelData.damageCorrection.Value : 0;
                levelSO.heartCorrection = levelData.heartCorrection.HasValue ? levelData.heartCorrection.Value : 0;
                levelSO.speedCorrection = levelData.speedCorrection.HasValue ? levelData.speedCorrection.Value : 0;
                levelSO.unlockedSkill = levelData.unlockedSkill.HasValue ? levelData.unlockedSkill.Value : 0;
                levelSO.unlockability = levelData.unlockedSkill.HasValue ? true : false;
                levelSO.correctability = levelData.damageCorrection.HasValue ? true : false;

                //SO 저장 - ID를 4자리 숫자로 포맷팅
                string assetPath = $"{outputFolder}/Level_{levelData.level}.asset";
                AssetDatabase.CreateAsset(levelSO, assetPath);

                //에셋 이름 저장
                levelSO.name = $"Level_{levelData.level}";
                createdLevels.Add(levelSO);

                EditorUtility.SetDirty(levelSO);
            }

            //데이터 베이스 생성
            if (creatDatabase && createdLevels.Count > 0)
            {
                LevelDatabaseSO database = ScriptableObject.CreateInstance<LevelDatabaseSO>();
                database.levels = createdLevels;

                AssetDatabase.CreateAsset(database, $"{outputFolder}/LevelDatabase.asset");
                EditorUtility.SetDirty(database);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Sucess", $"Created {createdLevels.Count} scriptable objects!", "OK");
        }
        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("Error", $"Failed to Convert JSON: {e.Message}", "OK");
            Debug.LogError($"JSON 변환 오류: {e}");
        }
    }
}
#endif