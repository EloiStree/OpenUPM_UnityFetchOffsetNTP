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

    public long m_timestampMilliseconds;
    public long m_timestampMillisecondsNTP;
    public string m_classicDateFromat= "yyyy-MM-dd HH:mm:ss.fff";
    public string m_ntpDate = "-";
    public string m_ntpTimeInSecond = "0";
    public string m_ntpTimeInMillisecond = "0";
    public string m_ntpTimeInMicrosecond = "0";
    public string m_ntpTimeInTicks = "0";
    public UnityEvent<string> m_onNtpTimeDateFormat;
    public UnityEvent<string> m_onIvp4Refresh;


    public void SetNtpServerNameTo(string nameNtpServer) {
        m_ntpServer = nameNtpServer; 
    }

    [ContextMenu("Set NTP as Be Pool")]
    public void SetNtpServerNameToBePoolNtp() => SetNtpServerNameTo("be.pool.ntp.org");

    [ContextMenu("Set NTP as Raspberry Pi")]
    public void SetNtpServerNameToRaspberryPi() => SetNtpServerNameTo("raspberrypi.local");
    [ContextMenu("Set NTP as Raspberry Pi 5")]
    public void SetNtpServerNameToRaspberryPi5() => SetNtpServerNameTo("raspberrypi5.local");
    [ContextMenu( "Set NTP as APInt IO Home")]
    public void SetNtpServerNameToAPIntIoHome() => SetNtpServerNameTo("apint-home.ddns.io");
    [ContextMenu("Set NTP as APInt IO Server")]
    public void SetNtpServerNameToAPIntIoServer() => SetNtpServerNameTo("apint.ddns.io");
    

    public void Awake()
    {
        string ipv4 = NtpOffsetFetcher.GetIpv4FromHostname(m_ntpServer);
        m_onIvp4Refresh.Invoke($"{m_ntpServer}({ipv4})");
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
        m_ntpOffsetLocalToServerInMilliseconds = 
            NtpOffsetFetcher.
            FetchNtpOffsetInMilliseconds(m_ntpServer
            , out bool hadError);
        if (hadError) {
            m_ntpOffsetLocalToServerInMilliseconds = 0;
        }
        m_onNtpOffsetRefreshInMilliseconds.Invoke(m_ntpOffsetLocalToServerInMilliseconds);
        m_onNtpOffsetRefreshMillisecondsAdjusted.Invoke(m_ntpOffsetLocalToServerInMilliseconds + m_manualAdjustement);
    }

    public void Update()
    {
        NtpOffsetFetcher.GetCurrentTimeAsMillisecondsLocalUtc(out m_timestampMilliseconds);
        NtpOffsetFetcher.GetCurrentTimeAsMillisecondsUtcNtp(
            m_timestampMilliseconds,
            m_ntpOffsetLocalToServerInMilliseconds,
            out m_timestampMillisecondsNTP);
        DateTime timeNtp = DateTimeOffset.FromUnixTimeMilliseconds(m_timestampMillisecondsNTP).DateTime;

        m_ntpTimeInSecond  = m_timestampMillisecondsNTP/1000 + "s";
        m_ntpTimeInMillisecond = m_timestampMillisecondsNTP + "ms";
        m_ntpTimeInMicrosecond = m_timestampMillisecondsNTP * 1000 + "µs";
        m_ntpTimeInTicks = m_timestampMillisecondsNTP * 10000 + "t";

        m_ntpDate = timeNtp.ToString(m_classicDateFromat);
        m_onNtpTimeDateFormat.Invoke(m_ntpDate);
    }

   
    public void GetCurrentTimeAsMillisecondsNtp(out long timeInMilliseconds)
    {
        long milliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        milliseconds += m_ntpOffsetLocalToServerInMilliseconds * 10000;
        timeInMilliseconds = milliseconds;

    }
    
}
