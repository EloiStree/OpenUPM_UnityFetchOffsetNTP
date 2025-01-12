using UnityEngine;
using UnityEngine.Events;

public class NTPMono_RelayOffsetMilliseconds : MonoBehaviour
{

    public long m_offsetMilliseconds = 0;
    public UnityEvent<long> m_onRelayMilliseconds;
    public void PushOffsetMilliseconds(int millisecondsOffset) { 
        long offset = millisecondsOffset;
        m_offsetMilliseconds = offset;
        m_onRelayMilliseconds.Invoke(offset);
    }
    public void PushOffsetMilliseconds(long millisecondsOffset) { 
        long offset = millisecondsOffset;
        m_offsetMilliseconds = offset;
        m_onRelayMilliseconds.Invoke(offset);
    }
   
}
