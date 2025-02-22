using UnityEngine;
using UnityEngine.Events;

public class NtpOffsetOnlyOnceMono : MonoBehaviour { 

    public string [] m_serversNtp  =new string[]{
        "raspberrypi.local",
        "be.pool.ntp.org" 
    } ;
    public long m_offsetInMilliseconds = 0;

    public bool m_loadAtAwake = true;

    public long m_timeInMilliseconds;
    public long m_timeInSeconds;

    public bool m_loadedOk;
    private long m_previousNtpOffset;
    public UnityEvent<long> m_onNtpOffsetChanged;
    public UnityEvent<long> m_onMillisecondsChanged;
    public UnityEvent<long> m_onSecondsChanged;

    public UnityEvent<string> m_onServerName;
    public UnityEvent<string> m_onServerIpv4;

    private void Awake()
    {
        if (m_loadAtAwake) {
            m_loadedOk= StaticNtpOffsetFetcher.UpdateOffsetFromServer(m_serversNtp,true);
            m_offsetInMilliseconds = StaticNtpOffsetFetcher.GetOffsetInMilliseconds();
            CheckForNtpOffsetChanged();
        }
    }
    public void CheckForNtpOffsetChanged() {

        if(m_previousNtpOffset != m_offsetInMilliseconds)
        {
            m_previousNtpOffset = m_offsetInMilliseconds;
            m_onNtpOffsetChanged.Invoke(m_offsetInMilliseconds);
        }
    }

    private long m_previousMilliseconds;
    private long m_previousSeconds;
    public void Update()
    {
        StaticNtpOffsetFetcher.GetCurrentTimeAsMillisecondsNtp(out m_timeInMilliseconds);
        m_offsetInMilliseconds = StaticNtpOffsetFetcher.GetOffsetInMilliseconds();
        m_timeInSeconds = m_timeInMilliseconds / 1000;
        CheckForNtpOffsetChanged();
        if (m_previousMilliseconds != m_timeInMilliseconds) 
        {
                m_previousMilliseconds = m_timeInMilliseconds;
                m_onMillisecondsChanged.Invoke(m_timeInMilliseconds);  
        }
        if (m_previousSeconds != m_timeInSeconds)
        { 
            m_previousSeconds = m_timeInSeconds;
            m_onSecondsChanged.Invoke(m_timeInSeconds);
            m_onServerName.Invoke(StaticNtpOffsetFetcher.m_serverNameUsed);
            m_onServerIpv4.Invoke(StaticNtpOffsetFetcher.m_serverIpv4Used);
        }

    }

    [ContextMenu("Force Ntp Offset Update")]
    public void ForceNtpUpdate() {

        m_loadedOk = StaticNtpOffsetFetcher.UpdateOffsetFromServerForce(m_serversNtp);
        m_offsetInMilliseconds = StaticNtpOffsetFetcher.GetOffsetInMilliseconds();
    }
}
