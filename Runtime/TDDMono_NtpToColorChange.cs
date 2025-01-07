using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class TDDMono_NtpToColorChange : MonoBehaviour
{

    public long m_ntpOffsetLocalToNtp;


    public void SetOffsetInTickLocalToServer(long offset)
    {
        m_ntpOffsetLocalToNtp = offset;
    }

    public double m_currentSecondsInDoube;
    public ulong m_secondsPerTicks = 3;

    public ulong m_currentSecondsNtp;
    public ulong m_previousSecondsNtp;



    public Camera m_camera;
    public UnityEvent<Color> m_colorEvent;
    public Color m_colorToChangeTo;
    public void Update()
    {

        DatetimeUtilityNTP.GetTimeNowUtcSeconds(m_ntpOffsetLocalToNtp, out m_currentSecondsInDoube);
        
        m_currentSecondsNtp = (ulong)m_currentSecondsInDoube;

        if (m_currentSecondsNtp != m_previousSecondsNtp) {
            m_previousSecondsNtp = m_currentSecondsNtp;
            if (m_currentSecondsNtp % m_secondsPerTicks == 0)
            {
                ulong i = m_currentSecondsNtp % (ulong)10000;
                System.Random random = new System.Random((int)i);
                float r = (float)random.NextDouble();   
                float g = (float)random.NextDouble();
                float b = (float)random.NextDouble();
                m_colorToChangeTo = new Color(
                    r,
                    g,
                    b,
                    1
                    );
                if (m_camera == null)
                    m_camera = Camera.main;
                if (m_camera != null)
                    m_camera.backgroundColor = m_colorToChangeTo;
                m_colorEvent.Invoke(m_colorToChangeTo);

            }
        }
    }

}
