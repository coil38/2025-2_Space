using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelDatabase", menuName = "Level/Database")]
public class LevelDatabaseSO : ScriptableObject
{
    public List<LevelSO> levels = new List<LevelSO>();

    //캐싱을 위한 사전
    private Dictionary<string, LevelSO> levelByKey;
    private Dictionary<int, LevelSO> levelByLevel;
    public int maxLevel;  //최대 레벨

    public void Initialize()  //초기화
    {
        levelByKey = new Dictionary<string, LevelSO>();
        maxLevel = levels.Count - 1; //데이터 베이스에 0레벨도 포함하기 때문에 -1 추가

        foreach (var level in levels)
        {
            levelByKey[level.levelKey] = level;
        }

        levelByLevel = new Dictionary<int, LevelSO>();
        foreach (var level in levels)
            levelByLevel[level.level] = level;
    }

    public LevelSO GetLevelByKey(string levelKey)  //스트링키로 레벨 찾기
    {
        if (levelByKey == null)
            Initialize();
        if(levelByKey.TryGetValue(levelKey, out LevelSO level))
            return level;

        return null;
    }

    public int GetMaxExp(int _level)
    {
        if (levelByKey == null)
            Initialize();
        foreach (var level in levelByKey)
        {
            if (level.Value.level == _level)
                return level.Value.maxEX;
        }

        return 0;
    }

    public LevelSO GetLevelByLevel(int _level)  //스트링키로 레벨 찾기
    {
        if (levelByLevel == null)
            Initialize();
        if (levelByLevel.TryGetValue(_level, out LevelSO level))
            return level;

        return null;
    }
}
