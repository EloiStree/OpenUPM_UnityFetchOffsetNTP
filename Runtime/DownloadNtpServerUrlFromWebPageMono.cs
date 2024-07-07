using System;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class DownloadNtpServerUrlFromWebPageMono : MonoBehaviour
{
    public string m_webpageContainingNtpServerAddress = "https://github.com/EloiStree/IP/blob/main/IIDWS/DEFAULT_NTP.txt";
    public UnityEvent<string> m_onWebPageAsStringDownloaded;


    public bool m_loadOnStart = true;

    private void Start()
    {
        if (m_loadOnStart)
        {
            DownloadWebPageAsString();
        }
    }

    [ContextMenu("Download Webpage as string")]
    public void DownloadWebPageAsString()
    {
        Task.Run(() => DownloadWebPageAsStringAsync());
    }

    async void DownloadWebPageAsStringAsync()
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                string content = await client.GetStringAsync(m_webpageContainingNtpServerAddress);
                m_onWebPageAsStringDownloaded.Invoke(content.Trim());
            }
            catch (Exception ex)
            {
                m_onWebPageAsStringDownloaded.Invoke("");
            }
        }

    }
}
