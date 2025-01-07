using System;
using System.Collections;
using System.Net;
using UnityEngine;
using UnityEngine.Events;

public class DateTimeNTPMono: MonoBehaviour
{
    //public string m_ntpServer= "time.windows.com";

    public UnityEvent<long> m_onUtcToNtpTickOffset;
    public string m_ntpServer = "pool.ntp.org";
    public string m_currentTimeOnPc;
    public string m_currentTimeOnNtp;
    public long m_currentTimeOnPcTick;
    public long m_currentTimeOnNtpTick;
    public long m_differencePcNtpTick;
    public double m_differencePcNtpMilliseconds;
    public DateTime m_currentTimeOnPcDate;
    public DateTime m_currentTimeOnNtpDate;
    public bool m_currentDateTimeUtcUseSummer;
    public bool m_currentDateTimeUtcNTPUseSummer;
    public DateTime GetAdjustedTime()
    {
        DateTime d = DateTime.UtcNow;
        return d.AddMilliseconds( m_differencePcNtpMilliseconds);
    }

    public bool m_autoRefresh = true;
    public float m_refreshRateInSeconds = 1;
    public int m_refreshCount = 10;
    public bool m_setGlobalNtpFromInspector = true;
    public bool m_failToReachNtpServer;

    private void Awake()
    {

        // check if 
        m_ntpServer = GetIpOfGivenServerName(m_ntpServer);


        if (m_setGlobalNtpFromInspector)
        SetGlobalNtpServer(m_ntpServer);
    }


    [ContextMenu("pool.ntp.org")]
    public void SetToPoolNtpOrgTarget()
    {
        m_ntpServer = "pool.ntp.org";
    }
    [ContextMenu("raspberrypi5.local")]
    public void SetToRaspberryPi5()
    {
        m_ntpServer = "raspberrypi5.local";
    }
    public string GetIpOfGivenServerName(string ntpServerName)
    {
        try
        {
            // Get the host entry for the NTP server name
            var hostEntry = Dns.GetHostEntry(ntpServerName);

            // Loop through the addresses and find the first IPv4 address
            foreach (var address in hostEntry.AddressList)
            {
                if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return address.ToString(); // Return the IPv4 address
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            // Handle errors (e.g., DNS resolution issues)
            Console.WriteLine($"Error: {ex.Message}");
            return null;
        }
    }


private IEnumerator Start()
    {

        if (m_autoRefresh)
        {
            for (int i = 0; i < m_refreshCount; i++)
            {
                Refresh();
                yield return new WaitForSeconds(m_refreshRateInSeconds);
            }
        }
    }


    public  void SetGlobalNtpServer(string ntpServer)
    {
        DateTimeNTP.SetGlobalNtpServer(ntpServer);
        m_ntpServer = DateTimeNTP.GetGlobalNtpServer();
    }
    public void SetGlobalNtpServerWithInspector()
    {
        SetGlobalNtpServer(m_ntpServer);
    }

    [ContextMenu("Refresh")]
    public void Refresh() {

        bool isIp = IPAddress.TryParse(m_ntpServer, out _);

        if (!isIp)
            m_ntpServer = GetIpOfGivenServerName(m_ntpServer);
       


        m_currentTimeOnPcDate = DateTime.UtcNow; 
        //TimeZoneInfo timeZone = TimeZoneInfo.Local;
        //m_currentDateTimeUtcUseSummer = timeZone.IsDaylightSavingTime(m_currentTimeOnPcDate);
        //m_currentDateTimeUtcNTPUseSummer = timeZone.IsDaylightSavingTime(m_currentTimeOnNtpDate);
        try
        {

            m_currentTimeOnNtpDate = DateTimeNTP.GetNetworkTime(m_ntpServer);
        }
        catch (Exception e)
        {
            m_failToReachNtpServer = true;
            Debug.LogError("Failed to reach NTP server: " + m_ntpServer);
            return;
        }

        m_currentTimeOnPc = m_currentTimeOnPcDate.ToString();
        m_currentTimeOnNtp = m_currentTimeOnNtpDate.ToString();
        m_currentTimeOnPcTick =m_currentTimeOnPcDate.Ticks ;
        m_currentTimeOnNtpTick= m_currentTimeOnNtpDate.Ticks;
        m_differencePcNtpTick = m_currentTimeOnNtpTick - m_currentTimeOnPcTick;
        m_differencePcNtpMilliseconds = m_differencePcNtpTick /(double) TimeSpan.TicksPerMillisecond;
        m_onUtcToNtpTickOffset.Invoke(m_differencePcNtpTick);

    }
}
