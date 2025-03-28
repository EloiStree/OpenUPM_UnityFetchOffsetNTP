using UnityEngine;
using UnityEngine.Events;

public class NtpOffsetMono_ListenToOnlyOnceOffsetChanged: MonoBehaviour
{
    public long m_millisecondsOffset;
    public UnityEvent<long> m_onMillisecondsOffsetLongChanged;
    public UnityEvent<int> m_onMillisecondsOffsetIntChanged;
    public UnityEvent<string> m_onMillisecondsOffsetStringChanged;

    private void OnEnable()
    {
        StaticNtpOffsetOnlyOnce.AddListenerToOffsetChanged(OffsetChanged);
    }
    private void OnDisable()
    {
        StaticNtpOffsetOnlyOnce.RemoveListenerToOffsetChanged(OffsetChanged);
    }

    private void OffsetChanged(long millisecondsOffset)
    {
        m_millisecondsOffset= millisecondsOffset;
        m_onMillisecondsOffsetLongChanged.Invoke(millisecondsOffset);
        m_onMillisecondsOffsetIntChanged.Invoke((int)millisecondsOffset);
        m_onMillisecondsOffsetStringChanged.Invoke(millisecondsOffset.ToString());
    }
}
