using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveAniCondition : MonoBehaviour
{
    private static bool isAnimating = false;
    public static void EndAni()
    {
        isAnimating = false;
    }

    public static void StartAni()
    {
        isAnimating = true;
    }

    public static bool IsAnimating()
    {
        return isAnimating;
    }

    public static bool IsResetAni()
    {
        if (!isAnimating)
        {
            return true;
        }
        return false;
    }
}
