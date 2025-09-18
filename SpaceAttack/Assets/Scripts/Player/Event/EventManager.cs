using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static PlayerEvent f_CorrectionValueEvent;    // 무기,스킬, 레벨 등의 보정수치를 받기 위한 이벤트
    void Awake()
    {
        f_CorrectionValueEvent = new PlayerEvent();
    }
}
