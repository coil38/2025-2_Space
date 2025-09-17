#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class LogUtilToggle
{
    [MenuItem("Build/Debug Log/Enable DEBUG_LOG")]
    public static void EnableDugLog()
    {
        SetDebugLogDefine(true);
    }

    [MenuItem("Build/Debug Log/Disable DEBUG_LOG")]
    public static void DisableDugLog()
    {
        SetDebugLogDefine(false);
    }

    private static void SetDebugLogDefine(bool enable)
    {
        var targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup).Split(";").ToList();

        if (enable && !defines.Contains("DEBUG_LOG"))
            defines.Add("DEBUG_LOG");
        else if (!enable)
            defines.RemoveAll(d => d == "DEBUG_LOG");

        PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, string.Join(";", defines));
    }
}
#endif