using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicEffectManager : MonoBehaviour
{
    private static Dictionary<int, RelicEffectDecorator> relicsEffects = new Dictionary<int, RelicEffectDecorator>();

    private static int atkUpId = 100;          //공격력 퍼센트 상승
    private static int healChanceUpId = 101;   //피격시, 일정확률로 체력회복
    private static int critChanceUpId = 102;   //치확 페센트 상승
    private static int coolDownId = 107;       //쿨타임 감소
    private static int speedUpId = 105;        //이속 퍼센트 증가
    private static int misChanceUpId = 104;    //회피 확률 증가
    private static int relicDropChanceUpId = 106; //유물 드랍 확룰 증가

    public static int _atkUpId
    {
        get { return atkUpId; }
        private set { atkUpId = value; }
    }
    public static int _healChanceUpId
    {
        get { return healChanceUpId; }
        private set { healChanceUpId = value; }
    }
    public static int _critChanceUpId
    {
        get { return critChanceUpId; }
        private set { critChanceUpId = value; }
    }
    public static int _coolDownId
    {
        get { return coolDownId; }
        private set { coolDownId = value; }
    }
    public static int _speedUpId
    {
        get { return speedUpId; }
        private set { speedUpId = value; }
    }
    public static int _misChanceUpId
    {
        get { return misChanceUpId; }
        private set { misChanceUpId = value; }
    }
    public static int _relicDropChanceUpId
    {
        get { return relicDropChanceUpId; }
        private set { relicDropChanceUpId = value; }
    }
    public void Initialize()  //초기화 함수
    {
        RelicEffectDecorator[] relicComps = GetComponents<RelicEffectDecorator>();
        foreach (var relic in relicComps)
        {
            relic.SetID();  //각 유물효과 ID 설정
            relicsEffects.Add(relic.relicEffectID, relic);
        }
    }

    public static RelicEffectDecorator GetRelicEffect(int id)
    {
        if (relicsEffects.Count <= 0)
        {
            LogUtil.LogError("유물효과 초기설정이 완료되지 않았습니다.");
            return null;
        }
        if (relicsEffects.TryGetValue(id, out RelicEffectDecorator effect))
        {
            return effect;
        }
        return null;
    }
}
