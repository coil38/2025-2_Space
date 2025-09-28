using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicFloatingUI : MonoBehaviour
{
    private bool isOnFloating = false;
    private BaseRelic currentRelic;
    public void SetFloatingUI(bool onFloatingText, BaseRelic relic = null)
    {
        gameObject.SetActive(onFloatingText);
        isOnFloating = onFloatingText;
        if (relic != null) currentRelic = relic;
    }

    private void Update()
    {
        if (isOnFloating)
        {
            Vector3 targetPos = currentRelic.gameObject.transform.position + Vector3.right * 0.6f;
            transform.position = targetPos;
        }
    }

}
