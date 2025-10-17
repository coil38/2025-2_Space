using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static PlayerEvent playerEvent;    // 무기,스킬, 레벨 등의 보정수치를 받기 위한 이벤트
    public static RelicEvent relicEvent;      //유물 관련 모든 조건 이벤트
    void Awake()
    {
        playerEvent = new PlayerEvent();
        relicEvent = new RelicEvent();
    }
}
