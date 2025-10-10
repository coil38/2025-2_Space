using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class SaveSystem
{
    private static string saveFolder = Application.persistentDataPath + "/Save/";

    //저장 슬롯 이름에 따른 파일 경로 변환
    private static string GetSavePath(string slotName)
    {
        return saveFolder + slotName + ".json";
    }

    //저장 데이터 직렬화 후 파일 저장
    public static void Save<T>(string slotName, T data)
    {
        try
        {
            if (!Directory.Exists(saveFolder))
                Directory.CreateDirectory(saveFolder);

            string json = JsonUtility.ToJson(data, true);

            //임시 파일에 먼저 저장 (중간에 끊기면 기존 파일 보존)
            string tempPath = GetSavePath(slotName) + ".temp";
            File.WriteAllText(tempPath, json);

            //기존 파일 삭제 후 교체
            if (File.Exists(GetSavePath(slotName)))
                File.Delete(GetSavePath(slotName));

            File.Move(tempPath, GetSavePath(slotName));
            //LogUtil.Log(GetSavePath(slotName));
        }
        catch (Exception e)
        {
            LogUtil.LogError($"Save failed: {e}");
        }
    }

    public static T Load<T>(string slotName) where T : new()
    {
        string path = GetSavePath(slotName);
        if (!File.Exists(path))
        {
            LogUtil.LogWarning($"Save file not found: {path}");
            return new T();
        }

        try
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<T>(json);
        }
        catch (Exception e)
        {
            LogUtil.LogError($"Load failed: {e}");
            return new T();
        }
    }

    //헤당 슬롯의 세이브 파일 삭제
    public static void Delete(string slotName)
    {
        string path = GetSavePath(slotName);
        if (File.Exists(path))
            File.Delete(path);
    }

    //특정 슬롯이 존재하는지 여부 확인
    public static bool Exists(string slotName)
    {
        return File.Exists(GetSavePath(slotName));
    }
}
