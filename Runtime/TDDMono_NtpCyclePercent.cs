using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TDDMono_NtpCyclePercent : MonoBehaviour
{

    public long m_ntpOffsetMilliseconds;

    public float m_cycleSeconds = 10;

    public long m_tickTimeNtp;

    public long m_cycleTicks;
    public long m_moduloOfTicksCycle;

    [Range(0,1)]
    public double m_percentOfCycle;
    public UnityEvent<double> m_onCyclePercentDoubleEvent;
    public UnityEvent<float> m_onCyclePercentFloatEvent;

    public void SetOffsetLocalToServerTicks(long offset)
    {
        m_ntpOffsetMilliseconds = offset;
    }


    public void Update()
    {
        DatetimeUtilityNTP.GetTimeNowTicksUtc(m_ntpOffsetMilliseconds, out  m_tickTimeNtp);
        m_cycleTicks = (long)(m_cycleSeconds * TimeSpan.TicksPerSecond);
        m_moduloOfTicksCycle = m_tickTimeNtp % m_cycleTicks;
        m_percentOfCycle = (double)m_moduloOfTicksCycle / (double)m_cycleTicks;
        m_onCyclePercentDoubleEvent.Invoke(m_percentOfCycle);
        m_onCyclePercentFloatEvent.Invoke((float)m_percentOfCycle);

    }
}
