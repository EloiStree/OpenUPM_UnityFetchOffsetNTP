using UnityEngine;
using Eloi.IID;
using UnityEngine.Events;
using System;

public class NtpOffsetFetcherMono : MonoBehaviour
{
    public string m_ntpServer = "be.pool.ntp.org";
    public int m_ntpOffsetLocalToServerInMilliseconds = 0;
    public UnityEvent<int> m_onNtpOffsetRefreshInMilliseconds;
    public int m_manualAdjustement = 0;
    public UnityEvent<int> m_onNtpOffsetRefreshMillisecondsAdjusted;

    public string m_classicDateFromat= "yyyy-MM-dd HH:mm:ss.fff";
    public string m_ntpDate = "-";
    public string m_ntpTimeInSecond = "0";
    public string m_ntpTimeInMillisecond = "0";
    public string m_ntpTimeInMicrosecond = "0";
    public string m_ntpTimeInTicks = "0";


    public void SetNtpServerNameTo(string nameNtpServer) {
        m_ntpServer = nameNtpServer; 
    }

    [ContextMenu("Set NTP as Raspberry Pi")]
    public void SetNtpServerNameToRaspberryPi() => SetNtpServerNameTo("raspberrypi.local");
    [ContextMenu("Set NTP as Raspberry Pi 5")]
    public void SetNtpServerNameToRaspberryPi5() => SetNtpServerNameTo("raspberrypi5.local");
    [ContextMenu( "Set NTP as APInt IO Home")]
    public void SetNtpServerNameToAPIntIoHome() => SetNtpServerNameTo("apint-home.ddns.io");
    [ContextMenu("Set NTP as APInt IO Server")]
    public void SetNtpServerNameToAPIntIoServer() => SetNtpServerNameTo("apint.ddns.io");
    public UnityEvent<string> m_onNtpTimeDateFormat;

    public void Awake()
    {
        Refresh();
    }
    public void Start()
    {
        Refresh();
    }
    public void OnEnable()
    {
        Refresh();
    }

    [ContextMenu("Refresh")]
    public void Refresh()
    {
        m_ntpOffsetLocalToServerInMilliseconds = NtpOffsetFetcher.FetchNtpOffsetInMilliseconds(m_ntpServer);
        m_onNtpOffsetRefreshInMilliseconds.Invoke(m_ntpOffsetLocalToServerInMilliseconds);
        m_onNtpOffsetRefreshMillisecondsAdjusted.Invoke(m_ntpOffsetLocalToServerInMilliseconds + m_manualAdjustement);
    }

    public void Update()
    {
        GetCurrentTime(out long time);
        m_ntpTimeInSecond = (time / 10000000).ToString();
        m_ntpTimeInMillisecond = (time / 10000).ToString();
        m_ntpTimeInMicrosecond = (time / 10).ToString();
        m_ntpTimeInTicks = time.ToString();
        DateTime d = new DateTime(time);
        m_ntpDate = d.ToString(m_classicDateFromat);
        m_onNtpTimeDateFormat.Invoke(m_ntpDate);
    }

    public void GetCurrentTime(out long time) { 
        long ticks = DateTime.UtcNow.Ticks;
        ticks +=m_ntpOffsetLocalToServerInMilliseconds * 10000;
        time = ticks;

    }
}
