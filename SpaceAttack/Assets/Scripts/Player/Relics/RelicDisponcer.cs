using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicDisponcer : MonoBehaviour
{
    public void DropRelicObjRandomly()
    {
        if (DataManager.instance == null || DataManager.instance._RelicDatabase.GetRelicCount() == 0)
        {
            LogUtil.LogError("DataManager 인스턴스가 생성되지 않거나 유물이 할당되지 않았습니다.");
            return;
        }

        RelicSO[] relics = DataManager.instance._RelicDatabase.GetRelics();
        int randomValue2 = Random.Range(0, relics.Length);
        RelicSO relic = DataManager.instance._RelicDatabase.GetRelicByIndex(randomValue2);  //받은 유물중, 랜덤index의 유물 받기

        GameObject temp = DataManager.instance._relicObject;
        GameObject relicObj = Instantiate(temp, transform.position + Vector3.up * 10, temp.transform.rotation);

        relicObj.GetComponent<BaseRelic>().Initialize(relic.relicID, relic.relicName, relic.iconSprite); //생성한 유물 오브젝트에 유물정보 갱신

        float randX = UnityEngine.Random.Range(-1f, 1f);
        float randZ = UnityEngine.Random.Range(-1f, 1f);

        Vector3 dir = new Vector3(randX, 1, randZ).normalized;
        float force = 3f;
        relicObj.GetComponent<Rigidbody>().AddForce(dir * force);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            DropRelicObjRandomly();
        }
    }
}
