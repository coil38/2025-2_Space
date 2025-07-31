using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinAnimationEventRelay : MonoBehaviour
{
    public CoinMonster parent;

    public void OnRollDone()
    {
        if (parent != null)
        {
            parent.OnRollDone();  // 부모의 함수 호출
        }
    }
    public void PlayRollSound()
    {
        if (parent != null)
        {
            parent.PlayRollSound();
        }
    }

    public void OnChargeStart()
    {
        parent.OnChargeStart();
    }

    public void OnExplode()
    {
        parent.OnExplode();
    }

    public void Explode()
    {
        parent.Explode();
    }
}
