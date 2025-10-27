using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveAniCondition : MonoBehaviour
{
    private static bool isAnimating = false;
    private static bool isResetingAni = false;
    public static void EndAni()
    {
        isAnimating = false;
        isResetingAni = true;
    }

    public static void StartAni()
    {
        isAnimating = true;
        isResetingAni = false;
    }

    public static bool IsAnimating()
    {
        return isAnimating;
    }

    public static bool IsResetAni()
    {
        if (isResetingAni)
        {
            isResetingAni = false;
            return true;
        }
        return isResetingAni;
    }
}
