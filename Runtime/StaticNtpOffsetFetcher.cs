using UnityEngine;
using Eloi.IID;
using System;

public class StaticNtpOffsetFetcher { 

    public static bool m_wasLoadedOnceFromServer = false;
    public static long m_offsetInMilliseconds = 0;

    public static string m_serverNameUsed = "";
    public static string m_serverIpv4Used = "";

    public static long GetOffsetInMilliseconds()
    {

        return m_offsetInMilliseconds;
    }
    public static void UpdateOffsetFromCustomCode(int offsetInMilliseconds)
    {
        UpdateOffsetFromCustomCode((long)offsetInMilliseconds);
    }
    public static void UpdateOffsetFromCustomCode(long offsetInMilliseconds)
    {
        m_offsetInMilliseconds = offsetInMilliseconds;
        SaveOffsetWithPlayerPrefs();
        LoadOffsetWithPlayerPrefs();
    }
    public static long GetOffsetFromPlayerPrefs()
    {
        LoadOffsetWithPlayerPrefs();
        return m_offsetInMilliseconds;

    }
    public static bool UpdateOffsetFromServerForce(string [] ntpServers)
    {
        m_wasLoadedOnceFromServer = false;
        return UpdateOffsetFromServer(ntpServers,false);
    }

    public static bool UpdateOffsetFromServerForce(string ntpServer)
    {
        m_wasLoadedOnceFromServer = false;
        return UpdateOffsetFromServer(ntpServer);
    }

    public static bool UpdateOffsetFromServer(string[] ntpServers, bool loadOnlyIfNotLoadYet=true) {

        foreach (var ntpServer in ntpServers)
        {
            if (loadOnlyIfNotLoadYet && m_wasLoadedOnceFromServer) {
                // Debug.Log("Already loaded once from server");
                return true;
            }
            bool ok =UpdateOffsetFromServer(ntpServer, loadOnlyIfNotLoadYet);
            if (ok)
                return true;
        }
        LoadOffsetWithPlayerPrefs();
        return false;
    }

    public static bool UpdateOffsetFromServer(string ntpServerIvp4, bool loadOnlyIfNotLoadYet = true)
    {
        if (loadOnlyIfNotLoadYet && m_wasLoadedOnceFromServer) {

            Debug.Log("Already laoded once from server");
            return true;
        }
        try
        {

            int offset = NtpOffsetFetcher.FetchNtpOffsetInMilliseconds(ntpServerIvp4, out bool hadError);
            if (hadError)
                return false;

            // Debug.Log($"Fetch {ntpServer} --> {ipv4} =  {offset}");

            UpdateOffsetFromCustomCode(offset);
            m_wasLoadedOnceFromServer = true;
            m_serverNameUsed = ntpServerIvp4;
            m_serverIpv4Used = ntpServerIvp4;
            return true;
        }
        catch (System.NullReferenceException e)
        {
            Debug.LogError($"Server is null ?: {ntpServerIvp4} {e}");

          
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error NTP Fetch: {ntpServerIvp4} {e}");
            
        }
        return false;
    }

    public static void GetCurrentTimeAsMillisecondsNtp(out long timeInMilliseconds)
    {
        long milliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        milliseconds += m_offsetInMilliseconds * 10000;
        timeInMilliseconds = milliseconds;
    }

    public static void SaveOffsetWithPlayerPrefs() {
        PlayerPrefs.SetString("NtpOffsetInMilliseconds", m_offsetInMilliseconds.ToString() );
    }

    public static void LoadOffsetWithPlayerPrefs() {
        string text = PlayerPrefs.GetString("NtpOffsetInMilliseconds", "0");
        long.TryParse(text, out m_offsetInMilliseconds);
        StaticNtpOffsetOnlyOnce.SetNotifyIfChanged(m_offsetInMilliseconds);
    }
}
