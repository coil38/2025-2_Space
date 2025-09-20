using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicFloatingUI : MonoBehaviour
{
    public void SetFloatingUI(bool onFloatingText, BaseRelic relic = null)
    {
        if (onFloatingText && relic != null)
        {
            gameObject.SetActive(true);
            //LogUtil.Log("플로팅 텍스트 활성화");

            Vector3 targetPos = relic.gameObject.transform.position + Vector3.right * 0.6f;
            transform.position = targetPos;
        }
        else if (!onFloatingText)
        {
            gameObject.SetActive(false);
            //LogUtil.Log("플로팅 텍스트 비활성화");
        }
    }

}
