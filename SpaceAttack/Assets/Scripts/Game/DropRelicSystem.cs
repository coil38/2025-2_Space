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
            if (DataManager.instance == null || DataManager.instance._RelicDatabase.GetRelicCount() == 0)
            {
                LogUtil.LogError("DataManager 인스턴스가 생성되지 않거나 유물이 할당되지 않았습니다.");
                return null;
            }

            RelicSO[] relics = DataManager.instance._RelicDatabase.GetRelics();
            int randomValue2 = Random.Range(0, relics.Length);
            RelicSO relic = DataManager.instance._RelicDatabase.GetRelicByIndex(randomValue2);  //받은 유물중, 랜덤index의 유물 받기

            GameObject temp = DataManager.instance._relicObject;
            GameObject relicObj = Instantiate(temp, dropPos, temp.transform.rotation);

            relicObj.GetComponent<BaseRelic>().Initialize(relic.relicID, relic.relicName, relic.iconSprite); //생성한 유물 오브젝트에 유물정보 갱신

            return relicObj;
        }

        LogUtil.Log("유물 드랍 실패");
        return null;  //null을 반환할 경우, 유물 드랍 실패
    }
}
