# if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class GuideData
{
    public int guideID;
    public string label;
    public string tabID;
}

public class SubGuideData
{
    public int subtabID;
    public string label;
    public string pageID;
}

public class PageGuideData
{
    public int pageID;
    public string label;
    public string description;
    public string spritePath;
}

public class GuideDataLoader : EditorWindow
{
    public static string outputFolder = "Assets/ScriptableObjects/Guide";

    public static string guidejsonFilePath = "";
    public static string subjsonFilePath = "";
    public static string pagejsonFilePath = "";

    public static void ConvertJsonToScriptableObjects()
    {
        if (!Directory.Exists(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
        }

        string guidejsonText = File.ReadAllText(guidejsonFilePath);
        string subjsonText = File.ReadAllText(subjsonFilePath);
        string pagejsonText = File.ReadAllText(pagejsonFilePath);

        try
        {
            //JSON 파싱
            List<GuideData> guidelists = JsonConvert.DeserializeObject<List<GuideData>>(guidejsonText);
            List<SubGuideData> sublists = JsonConvert.DeserializeObject<List<SubGuideData>>(subjsonText);
            List<PageGuideData> pagelists = JsonConvert.DeserializeObject<List<PageGuideData>>(pagejsonText);

            List<GuideSO> createdGuids = new List<GuideSO>();

            foreach (var page in pagelists)
            {
                GuideSO guidSO = ScriptableObject.CreateInstance<GuideSO>();

                foreach (SubGuideData sub in sublists)
                {
                    string[] pageIds = sub.pageID.Split(',');
                    foreach (string pageId in pageIds)
                    {
                        if (int.TryParse(pageId.Trim(), out int result))
                        {
                            if (result == page.pageID)
                            {
                                guidSO.subId = sub.subtabID;   //가이드의 서브Id 갱신
                                guidSO.subTitle = sub.label;   //가이드의 서브label 갱신
                            }
                        }
                    }
                }

                foreach (GuideData main in guidelists)
                {
                    string[] subIds = main.tabID.Split(",");
                    foreach (string subId in subIds)
                    {
                        if (int.TryParse(subId.Trim(), out int result))
                        {
                            if (result == guidSO.subId)
                            {
                                guidSO.mainId = main.guideID;   //가이드의 메인Id 갱신
                                guidSO.mainTitle = main.label; //가이드의 메인label 갱신
                            }
                        }
                    }
                }

                //데이터 복사
                guidSO.pageId = page.pageID;
                guidSO.pageTitle = page.label;
                guidSO.description = page.description;

                if (!string.IsNullOrEmpty(page.spritePath))
                {
                    guidSO.pageSprite = AssetDatabase.LoadAssetAtPath<Sprite>(page.spritePath);

                    if(guidSO.pageSprite == null)
                        LogUtil.LogWarning($"{page.label}의 {page.spritePath}위치에 이미지가 존재하지 않음");
                }

                //SO 저장
                string assetPath = $"{outputFolder}/Guide_{page.label}.asset";
                AssetDatabase.CreateAsset(guidSO, assetPath);

                //에셋 이름 저장
                guidSO.name = $"Guide_{page.label}";
                createdGuids.Add(guidSO);

                EditorUtility.SetDirty(guidSO);
            }

            //데이터 베이스 생성
            if (createdGuids.Count > 0)
            {
                GuideDatabaseSO database = ScriptableObject.CreateInstance<GuideDatabaseSO>();
                database.guidList = createdGuids;

                AssetDatabase.CreateAsset(database, $"{outputFolder}/GuideDatabase.asset");
                EditorUtility.SetDirty(database);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Sucess", $"Created {createdGuids.Count} scriptable objects!", "OK");
        }
        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("Error", $"Failed to Convert JSON : {e.Message}", "OK");
            LogUtil.LogError($"JSON 변환 오류: {e}");
        }
    }
}
#endif