using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropRelicSystem : MonoBehaviour
{
    public static float defualtDropRate = 0.05f;
    public static float dropRate = 0.05f;
    public static GameObject DropRelicObjRandomly(Vector3 dropPos)
    {
        float randomValue = Random.value;
        if (randomValue <= dropRate)  //5퍼센트 확률
        {
            if (DataManager.instance == null || DataManager.instance._relics.Length == 0)
            {
                LogUtil.LogError("DataManager 인스턴스가 생성되지 않거나 유물이 할당되지 않았습니다.");
                return null;
            }

            GameObject[] relics = DataManager.instance._relics;
            int randomValue2 = Random.Range(0, relics.Length - 1);

            return Instantiate(relics[randomValue2], dropPos, relics[randomValue2].transform.rotation);
        }

        LogUtil.Log("유물 드랍 실패");
        return null;  //null을 반환할 경우, 유물 드랍 실패
    }
}
