using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TimerEvent : MonoBehaviour
{
    private static List<TimerEventData> timerEventDatas = new List<TimerEventData>();

    public static void Add(float time, Action action)
    {
        Timer timer = new Timer(time);
        timer.Start();
        TimerEventData data = new TimerEventData(timer, action);
        timerEventDatas.Add(data);
    }

    public static void Remove(Action action)
    {
        TimerEventData data = timerEventDatas.Find(t => t.timer.IsRunning() && t.action == action);
        if (data != null)
            timerEventDatas.Remove(data);
    }

    private void Update()
    {
        if (timerEventDatas.Count <= 0) return;

        foreach (var timerEvent in timerEventDatas.ToList())
        {
            timerEvent.timer.Update();
            if (!timerEvent.timer.IsRunning())
            {
                timerEvent.action?.Invoke();
                timerEventDatas.Remove(timerEvent);

                LogUtil.Log("버프효과 종료");
            }
        }
    }
}
public class TimerEventData
{
    public Timer timer;
    public Action action;

    public TimerEventData(Timer timer, Action action)
    {
        this.timer = timer;
        this.action = action;
    }
   
}