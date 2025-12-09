using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMoveAniCondition : MonoBehaviour
{
    private static bool isAnimating = false;
    private static bool isReset = false;
    public static void EndAni()
    {
        isAnimating = false;
        isReset = true;
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
        if (isReset)
        {
            isReset = false;
            return true;
        }
        return false;
    }
}
