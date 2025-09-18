# if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class RelicDataLoader : ScriptableObject
{
    private static string outputFolder = "Assets/ScriptableObjects/Relic";
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
            List<RelicData> relicList = JsonConvert.DeserializeObject<List<RelicData>>(jsonText);
            List<RelicSO> createdRelics = new List<RelicSO>();

            foreach (var relic in relicList)
            {
                RelicSO relicSO = ScriptableObject.CreateInstance<RelicSO>();

                //데이터 복사
                relicSO.relicID = relic.relicID;
                relicSO.relicName = relic.name;
                relicSO.darkMaterialCount = relic.darkMaterialCount;
                relicSO.description = relic.description;

                //유물 효과 대상 설정
                if (!string.IsNullOrEmpty(relic.relicEffects))
                {
                    string[] temps = relic.relicEffects.Split(",");
                    int[] temps2 = new int[temps.Length];
                    for (int i = 0; i < temps.Length; i++)
                    {
                        if (int.TryParse(temps[i].Trim(), out int res))
                        {
                            temps2[i] = res;
                        }
                    }
                    relicSO.relicEffects = temps2;
                }

                //유물 효과정보 추가
                if (!string.IsNullOrEmpty(relic.relicEffectInfo))
                {
                    string[] m_temps = relic.relicEffectInfo.Trim().Split("]");
                    List<RelicInfo> infos = new List<RelicInfo>();

                    //LogUtil.Log(string.Join(",", m_temps));
                    //LogUtil.Log("개수 :" + m_temps.Length);
                    for (int i = 0; i < m_temps.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(m_temps[i])) continue;

                        m_temps[i] = m_temps[i].Replace("[", string.Empty).Replace("]", string.Empty);
                        string[] s_temps = m_temps[i].Split(",");
                        float[] f_temps = new float[s_temps.Length];
                        for (int j = 0; j < s_temps.Length; j++)
                        {
                            if (float.TryParse(s_temps[j], out float result))
                            {
                                f_temps[j] = result;
                            }
                        }
                        switch (f_temps.Length)
                        {
                            case 2:
                                //LogUtil.Log($"{f_temps[0]},{f_temps[1]}");
                                if (f_temps[1] > Mathf.FloorToInt(f_temps[1]))    //소수점이 있을 경우
                                    infos.Add(new RelicInfo((int)f_temps[0], f_temps[1]));
                                else infos.Add(new RelicInfo((int)f_temps[0],0, (int)f_temps[1]));
                            break;
                            case 3:
                                //LogUtil.Log($"{f_temps[0]},{f_temps[1]},{f_temps[2]}");
                                infos.Add(new RelicInfo((int)f_temps[0], f_temps[1], (int)f_temps[2]));
                            break;
                        }
                    }

                    relicSO.relicInfos = infos.ToArray();
                }

                if (!string.IsNullOrEmpty(relic.iconPath))
                {
                    relicSO.iconSprite = AssetDatabase.LoadAssetAtPath<Sprite>(iconPath + relic.iconPath);

                    if (relicSO.iconSprite == null)
                        LogUtil.LogWarning($"{relic.name}이름의 {relic.iconPath}위치에 아이콘이 존재하지 않음");
                }

                //SO 저장
                string assetPath = $"{outputFolder}/{relic.name}_Icon.asset";
                AssetDatabase.CreateAsset(relicSO, assetPath);

                //에셋 이름 저장
                relicSO.relicName = $"Relic_{relic.name}";
                createdRelics.Add(relicSO);

                EditorUtility.SetDirty(relicSO);
            }

            //데이터 베이스 생성
            if (createDatabase && createdRelics.Count > 0)
            {
                RelicDatabaseSO database = ScriptableObject.CreateInstance<RelicDatabaseSO>();
                database.relics = createdRelics;

                AssetDatabase.CreateAsset(database, $"{outputFolder}/RelicDatabase.asset");
                EditorUtility.SetDirty(database);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Sucess", $"Created {createdRelics.Count} scriptable objects!", "OK");
        }
        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("Error", $"Failed to Convert JSON : {e.Message}", "OK");
            LogUtil.LogError($"JSON 변환 오류: {e}");
        }
    }
}
#endif
