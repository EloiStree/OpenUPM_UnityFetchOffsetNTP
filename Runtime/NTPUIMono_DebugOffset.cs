using Eloi.IID;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

public class NTPUIMono_DebugOffset : MonoBehaviour
{

    public int m_ntpOffsetLocalToServerMilliseconds = 0;
    public string m_dateFormat = "yyyy-MM-dd HH:mm:ss.fff";
    public long m_localTimeNoneUTCInMilliseconds = 0;
    public long m_localTimeInMillisecondsUTC = 0;
    public long m_ntpTimeInMillisecondsUTC = 0;
    public string m_localTimeNoneUTCInMillisecondsStr = "";
    public string m_localTimeInMillisecondsUTCStr = "";
    public string m_ntpTimeInMillisecondsUTCStr = "";

    public Events m_events = new Events();
    [System.Serializable]
    public class Events {

        public UnityEvent<string> m_onTextDebug;
    }

    public void SetOffsetMilliseconds(int offset)
    {
        m_ntpOffsetLocalToServerMilliseconds = offset;
    }

    void Update()
    {

        NtpOffsetFetcher.GetTimesFromOffsetMilliseconds(
            m_ntpOffsetLocalToServerMilliseconds,
            out m_localTimeNoneUTCInMilliseconds,
            out m_localTimeInMillisecondsUTC,
            out m_ntpTimeInMillisecondsUTC,
            out m_localTimeNoneUTCInMillisecondsStr,
            out m_localTimeInMillisecondsUTCStr,
            out m_ntpTimeInMillisecondsUTCStr,
            m_dateFormat
            );

        string text = $@"
 Offset: {m_ntpOffsetLocalToServerMilliseconds}
 NTPTimeUTC: {m_ntpTimeInMillisecondsUTC}
 LocalTimeNoneUTC: {m_localTimeNoneUTCInMillisecondsStr}
 LocalTimeUTC: {m_localTimeInMillisecondsUTCStr}
 NTPTimeUTC: {m_ntpTimeInMillisecondsUTCStr}
 ";
        m_events.m_onTextDebug.Invoke(text);

    }
}
