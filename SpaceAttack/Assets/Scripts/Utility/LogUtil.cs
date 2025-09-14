using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LogUtil
{
    [System.Diagnostics.Conditional("DEBUG_LOG")]
    public static void Log(object message)
    {
        Debug.Log(message);
    }

    [System.Diagnostics.Conditional("DEBUG_LOG")]
    public static void LogWarning(object message)
    {
        Debug.LogWarning(message);
    }

    [System.Diagnostics.Conditional("DEBUG_LOG")]
    public static void LogError(object message)
    {
        Debug.LogError(message);
    }
}
