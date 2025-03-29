

using System;
using System.Net.Sockets;
using System.Net;
using System.Linq;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;

namespace Eloi.IID
{

    public static class NtpOffsetFetcher
    {
        private static int sm_defaultGlobalNtpOffsetInMilliseconds = 0;
        private static int sm_connectServerTimeoutMilliseconds = 800;

        public static int FetchNtpOffsetInMilliseconds(string ntpServerIpv4, out bool hadError, int ntpPort=123)
        {
            bool isLocalRaspberryPi = ntpServerIpv4.Contains(".local");
            bool isAndroid= UnityEngine.Application.platform == UnityEngine.RuntimePlatform.Android;
            if (isLocalRaspberryPi && isAndroid)
            {
                UnityEngine.Debug.LogWarning("Raspberry PI .local is mDNS that is not supported on Quest and some devices (" + ntpServerIpv4 + "). You can use https://github.com/EloiStree/2025_03_26_ScanForRaspberryPi if youp want to do LAN Raspberryi PI");
            }



            string [] split = ntpServerIpv4.Split(new char[] { '.',':'});
            bool isIpv4 = split.Length>=4 
                && int.TryParse(split[0], out int _)
                && int.TryParse(split[1], out int _)
                && int.TryParse(split[2], out int _)
                && int.TryParse(split[3], out int _);
            bool hasItOwnPort = isIpv4 &&  split.Length == 5
                && int.TryParse(split[4], out ntpPort);

            if (hasItOwnPort)
                ntpServerIpv4 = $"{split[0]}.{split[1]}.{split[2]}.{split[3]}";

            hadError = false;
            try
            {
                var ntpData = new byte[48];
                ntpData[0] = 0x1B;

                IPAddress[] addresses = null; 
                if(!isIpv4)
                {
                    addresses = System.Net.Dns.GetHostEntry(ntpServerIpv4).AddressList;
                    UnityEngine.Debug.Log($"NTP Fetch: {ntpServerIpv4} {string.Join(",", addresses.Select(a => a.ToString()))}");
                }
                else
                {
                    addresses = new IPAddress[] { IPAddress.Parse(ntpServerIpv4) };
                    UnityEngine.Debug.Log($"NTP Fetch IPV4: {ntpServerIpv4} {string.Join(",", addresses.Select(a => a.ToString()))}");
                }
                var ipEndPoint = new System.Net.IPEndPoint(addresses[0], ntpPort);
                using (var socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Dgram, System.Net.Sockets.ProtocolType.Udp))
                {
                    socket.Connect(ipEndPoint);
                    socket.Send(ntpData);
                    socket.ReceiveTimeout = sm_connectServerTimeoutMilliseconds; // Set timeout to 5 seconds
                    try
                    {
                        socket.Receive(ntpData);
                    }
                    catch (SocketException ex)
                    {
                        UnityEngine.Debug.Log($"Socket timeout: {ex.Message}");
                        hadError = true;
                        return 0;
                    }
                }

                ulong intPart = BitConverter.ToUInt32(ntpData, 40);
                ulong fractPart = BitConverter.ToUInt32(ntpData, 44);
                intPart = SwapEndianness(intPart);
                fractPart = SwapEndianness(fractPart);
                var milliseconds = (intPart * 1000 + (fractPart * 1000) / 0x100000000L);
                var networkDateTime = (new DateTime(1900, 1, 1)).AddMilliseconds((long)milliseconds);
                var offset = (networkDateTime - DateTime.UtcNow).TotalMilliseconds;

                return (int)offset;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log ($"Error NTP Fetch: {ntpServerIpv4} {e}");
                hadError = true;
                return 0;
            }
        }

        public static void SetGlobalNtpOffsetInMilliseconds(string ntpServer)
        {
            try
            {
                var offset = FetchNtpOffsetInMilliseconds(ntpServer, out bool hadError);
                if( hadError) {
                    sm_defaultGlobalNtpOffsetInMilliseconds = 0;
                    return;
                }
                sm_defaultGlobalNtpOffsetInMilliseconds = offset;
                Console.WriteLine($"Default Global NTP Offset: {sm_defaultGlobalNtpOffsetInMilliseconds} {ntpServer}");
            }
            catch (Exception)
            {
                sm_defaultGlobalNtpOffsetInMilliseconds = 0;
            }
        }

        public static int GetGlobalNtpOffsetInMilliseconds()
        {
            return sm_defaultGlobalNtpOffsetInMilliseconds;
        }

        private static uint SwapEndianness(ulong x)
        {
            return (uint)(((x & 0x000000ff) << 24) + ((x & 0x0000ff00) << 8) + ((x & 0x00ff0000) >> 8) + ((x & 0xff000000) >> 24));
        }

        public static void GetTimesFromOffsetMilliseconds(
            int ntpOffsetLocalToServerMilliseconds,
            out long localTimeInMillisecondsNoneUTC, 
            out long localTimeInMillisecondsUTC,
            out long ntpTimeInMillisecondsUTC)
        {
            NtpOffsetFetcher.GetCurrentTimeAsMillisecondsLocalNoneUtc(out  localTimeInMillisecondsNoneUTC);
            NtpOffsetFetcher.GetCurrentTimeAsMillisecondsLocalUtc(out localTimeInMillisecondsUTC);
            NtpOffsetFetcher.GetCurrentTimeAsMillisecondsUtcNtp(
                localTimeInMillisecondsUTC,
                ntpOffsetLocalToServerMilliseconds,
                out ntpTimeInMillisecondsUTC);

        }
        public static string GetIpv4FromHostname(string hostname)
        {
            try
            {
                // Get the IP addresses associated with the hostname
                IPAddress[] addresses = Dns.GetHostAddresses(hostname);

                foreach (IPAddress address in addresses)
                {
                    // Check if the address is IPv4
                    if (address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return address.ToString();
                    }
                }
            }
            catch (System.Exception ex)
            {

                UnityEngine.Debug.LogWarning("Error resolving IP address in NtpOffsetFetcher NTP: " + ex.Message + "\n" + ex.StackTrace);
            }

            return "";
        }


        public static DateTime GetDateTimeFromTimestampMillisecondsUTC(long timestampMilliseconds)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(timestampMilliseconds).DateTime;
        }

        public static void GetTimesFromOffsetMilliseconds(
           int ntpOffsetLocalToServerMilliseconds,
            out long localTimeInMillisecondsNoneUTC,
            out long localTimeInMillisecondsUTC,
            out long ntpTimeInMillisecondsUTC,
            out string localTimeNoneUTCInMillisecondsStr,
            out string localTimeInMillisecondsUTCStr,
            out string ntpTimeInMillisecondsUTCStr,
           string dateFormat="yyyy-MM-dd ")
        {

            GetTimesFromOffsetMilliseconds(
                ntpOffsetLocalToServerMilliseconds,
                out localTimeInMillisecondsNoneUTC,
                out localTimeInMillisecondsUTC,
                out ntpTimeInMillisecondsUTC
                );
            localTimeNoneUTCInMillisecondsStr =
                GetDateTimeFromTimestampMillisecondsUTC(localTimeInMillisecondsNoneUTC).ToString(dateFormat);
            localTimeInMillisecondsUTCStr = 
                GetDateTimeFromTimestampMillisecondsUTC(localTimeInMillisecondsUTC).ToString(dateFormat);
            ntpTimeInMillisecondsUTCStr = 
                GetDateTimeFromTimestampMillisecondsUTC(ntpTimeInMillisecondsUTC).ToString(dateFormat);
        }

        //public static string GetIpv4FromHostname(string hostname)
        //{
        //    try
        //    {
        //        // Get the IP addresses associated with the hostname
        //        IPAddress[] addresses = Dns.GetHostAddresses(hostname);

        //        foreach (IPAddress address in addresses)
        //        {
        //            // Check if the address is IPv4
        //            if (address.AddressFamily == AddressFamily.InterNetwork)
        //            {
        //                return address.ToString();
        //            }
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
                
        //        UnityEngine.Debug.LogWarning("Error resolving IP address: " + ex.Message + "\n"+ex.StackTrace);
        //    }

        //    return null; 
        //}


        private static void GetCurrentTimeAsMillisecondsLocalNoneUtc(out long localTimeNoneUTCInMilliseconds)
        {
            long milliseconds = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            localTimeNoneUTCInMilliseconds = milliseconds;
        }

        public static void GetCurrentTimeAsMillisecondsLocalUtc(out long timeInMilliseconds)
        {
            long milliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            timeInMilliseconds = milliseconds;
        }

        public static void GetCurrentTimeAsMillisecondsNtp(long offsetMilliseconds, out long timeInMilliseconds)
        {
            GetCurrentTimeAsMillisecondsLocalUtc(out long localMilliseconds);
            GetCurrentTimeAsMillisecondsUtcNtp(localMilliseconds, offsetMilliseconds, out timeInMilliseconds);

        }

        public static void GetCurrentTimeAsMillisecondsUtcNtp(long timestampMilliseconds, long offsetMilliseconds, out long timeInMilliseconds)
        {
            long milliseconds = timestampMilliseconds;
            milliseconds += offsetMilliseconds ;
            timeInMilliseconds = milliseconds;

        }
    } 
}


    

    
    
  

    
        
        
            
            
        
        
        
        
 
        
        
        
        
        
        
        
        
        
        
        

    
        
   
        
   
        
        
    
     


    
    



        

        
                
        
        
        
        



        
    
    
            
    
        
        
                    
                    
                    
                    
                    
       
       
       
    
        
            
            


                








        
        
        
        
        
        
        
    
        
    




















































    
    
            
            
        
    


            
        
        
                


            
            
        
    

        


   

        
 

 

        
    



        
        























    
        


      
    


        
            
            
            