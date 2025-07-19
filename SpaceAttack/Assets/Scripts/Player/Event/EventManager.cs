using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static FindCorectionValueEvent f_CorrectionValueEvent;    // 무기,스킬의 보정수치를 받기 위한 이벤트
    void Awake()
    {
        f_CorrectionValueEvent = new FindCorectionValueEvent();
    }
}
