using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventRelay : MonoBehaviour
{
    public ShoeMonster parentScript;

    public void FireSingleBullet()
    {
        if (parentScript != null)
            parentScript.FireSingleBullet();
    }
}
