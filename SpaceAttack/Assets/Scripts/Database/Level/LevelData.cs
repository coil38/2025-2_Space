using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelData
{
    public string levelKey;
    public int level;
    public int maxEX;
    public int? damageCorrection;
    public int? heartCorrection;
    public int? speedCorrection;
    public int? unlockedSkill;
}
