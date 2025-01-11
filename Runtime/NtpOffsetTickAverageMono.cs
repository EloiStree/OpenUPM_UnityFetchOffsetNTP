using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NtpOffsetTickAverageMono : MonoBehaviour
{
    public NtpOffsetTickAverage m_offsetAverage;
    public UnityEvent<long> m_onOffsetTickChanged;

    public void PushNewOffset(int tickOffset)
    {
        PushNewOffset((long)tickOffset);
    }
    public void PushNewOffset(long tickOffset)
    {
        m_offsetAverage.PushNewOffset(tickOffset);
        m_onOffsetTickChanged.Invoke(m_offsetAverage.GetAverage());
    }
}




[System.Serializable]
public class NtpOffsetTickAverage
{

    public long m_lastOffsetTickReceived;
    public long m_minimumOffsetTick;
    public long m_maximumOffsetTick;
    public long m_averageOffsetTick;

    private static long m_lastAverageOffsetTick;
    public static long GetLastAverageOffsetTickComputed()
    {
        return m_lastAverageOffsetTick;
    }

    public int m_historySize = 10;
    public List<long> m_historyOffsetTicks = new List<long>();
    public void PushNewOffset(long tickOffset)
    {
        if (m_minimumOffsetTick == 0)
        {
            m_minimumOffsetTick = tickOffset;
        }
        if (m_maximumOffsetTick == 0)
        {
            m_maximumOffsetTick = tickOffset;
        }
        if (m_averageOffsetTick == 0)
        {
            m_averageOffsetTick = tickOffset;
        }

        m_lastOffsetTickReceived = tickOffset;
        m_historyOffsetTicks.Add(tickOffset);
        if (m_historyOffsetTicks.Count > m_historySize)
        {
            m_historyOffsetTicks.RemoveAt(0);
        }
        if (m_minimumOffsetTick > tickOffset)
        {
            m_minimumOffsetTick = tickOffset;
        }
        if (m_maximumOffsetTick < tickOffset)
        {
            m_maximumOffsetTick = tickOffset;
        }
        m_averageOffsetTick = GetAverage();
        m_lastAverageOffsetTick = m_averageOffsetTick;
    }

    public long GetAverage()
    {
        long sum = 0;
        foreach (var offset in m_historyOffsetTicks)
        {
            sum += offset;
        }
        return sum / m_historyOffsetTicks.Count;
    }
}