using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ExpInfo
{
    public int currentExp { get; private set; }
    public int targetExp { get; private set; }
    public int maxExp { get; private set; }
    public int currentLevel { get; private set; }
    public int nextMaxExp { get; private set; }

    public ExpInfo(int _currentExp, int _targetExp, int _maxExp, int _currentLevel, int _nextMaxExp)
    {
        currentExp = _currentExp;
        targetExp = _targetExp;
        maxExp = _maxExp;
        currentLevel = _currentLevel;
        nextMaxExp = _nextMaxExp;
    }
}
