using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "new Level", menuName = "Level/levels")]
public class LevelSO : ScriptableObject
{
    public string levelKey;
    public int level;
    public int maxEX;
    public float damageCorrection;
    public int heartCorrection;
    public float darkMatCountCorrection;
    public int unlockedSkill;
    public bool unlockability;
    public bool correctability;

    public override string ToString()
    {
        return $"[{levelKey}] 레벨: ({level}) 최대경험치: {maxEX}, 보정치: {damageCorrection}, {heartCorrection}, {darkMatCountCorrection}, 해금대상: {unlockedSkill}";
    }
}
