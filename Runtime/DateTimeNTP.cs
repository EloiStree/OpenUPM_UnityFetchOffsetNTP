using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
public class DateTimeNTP
{

    public static string m_globalServerNtpUrl = "pool.ntp.org";

    public static DateTime GetNetworkTime(string ntpServerUrl
        , out DateTime beforeConnection
        , out DateTime afterConnection
        , out DateTime networkDateTime
        , long tickBeforeToAfterConnection
        , long ticksOfNetworkDateTime
        )
    {
        var ntpData = new byte[48];
        ntpData[0] = 0x1B; //LeapIndicator = 0 (no warning), VersionNum = 3 (IPv4 only), Mode = 3 (Client Mode)

        var addresses = Dns.GetHostEntry(ntpServerUrl).AddressList;
        var ipEndPoint = new IPEndPoint(addresses[0], 123);
        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        beforeConnection = DateTime.UtcNow;
        socket.Connect(ipEndPoint);
        socket.Send(ntpData);
        socket.Receive(ntpData);
        socket.Close();
        afterConnection = DateTime.UtcNow;

        ulong intPart = (ulong)ntpData[40] << 24 | (ulong)ntpData[41] << 16 | (ulong)ntpData[42] << 8 | (ulong)ntpData[43];
        ulong fractPart = (ulong)ntpData[44] << 24 | (ulong)ntpData[45] << 16 | (ulong)ntpData[46] << 8 | (ulong)ntpData[47];

        var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
        networkDateTime  = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);
        tickBeforeToAfterConnection = afterConnection.Ticks - beforeConnection.Ticks;
        ticksOfNetworkDateTime = networkDateTime.Ticks;
        return networkDateTime;
    }



    public static DateTime GetNetworkTime(string ntpServerUrl)
    {
        var ntpData = new byte[48];
        ntpData[0] = 0x1B; //LeapIndicator = 0 (no warning), VersionNum = 3 (IPv4 only), Mode = 3 (Client Mode)

        var addresses = Dns.GetHostEntry(ntpServerUrl).AddressList;
        var ipEndPoint = new IPEndPoint(addresses[0], 123);
        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        socket.Connect(ipEndPoint);
        socket.Send(ntpData);
        socket.Receive(ntpData);
        socket.Close();

        ulong intPart = (ulong)ntpData[40] << 24 | (ulong)ntpData[41] << 16 | (ulong)ntpData[42] << 8 | (ulong)ntpData[43];
        ulong fractPart = (ulong)ntpData[44] << 24 | (ulong)ntpData[45] << 16 | (ulong)ntpData[46] << 8 | (ulong)ntpData[47];

        var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
        var networkDateTime = (new DateTime(1900, 1, 1,0,0,0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);

        return networkDateTime;
    }

    public static void GetUtcNtpOffsetTick(out long tickOffset)
    {
        tickOffset  = 0;
    }


    public static void SetGlobalNtpServer(string ntpServer)
    {
        m_globalServerNtpUrl = ntpServer;
        Debug.Log("NTP Server set to: " + m_globalServerNtpUrl);
    }
    public static DateTime GetNetworkTime()
    {
        return GetNetworkTime(m_globalServerNtpUrl);
    }
    public static void GetUtc2NtpOffsetTick(out long tickOffset)
    {
        DateTime networkDateTime = GetNetworkTime();
        DateTime localDateTime = DateTime.UtcNow;
        tickOffset = networkDateTime.Ticks - localDateTime.Ticks;
    }
    public static string GetGlobalNtpServer()
    {
        return m_globalServerNtpUrl;
    }
}
