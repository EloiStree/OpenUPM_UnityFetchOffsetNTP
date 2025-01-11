

using System;
using System.Security.Policy;

namespace Eloi.IID
{

    public static class NtpOffsetFetcher
    {
        private static int defaultGlobalNtpOffsetInMilliseconds = 0;

        public static int FetchNtpOffsetInMilliseconds(string ntpServer)
        {
            try
            {
                var ntpData = new byte[48];
                ntpData[0] = 0x1B;
                var addresses = System.Net.Dns.GetHostEntry(ntpServer).AddressList;
                var ipEndPoint = new System.Net.IPEndPoint(addresses[0], 123);
                using (var socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Dgram, System.Net.Sockets.ProtocolType.Udp))
                {
                    socket.Connect(ipEndPoint);
                    socket.Send(ntpData);
                    socket.Receive(ntpData);
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
                Console.WriteLine($"Error NTP Fetch: {ntpServer} {e}");
                return 0;
            }
        }

        public static void SetGlobalNtpOffsetInMilliseconds(string ntpServer)
        {
            try
            {
                var offset = FetchNtpOffsetInMilliseconds(ntpServer);
                defaultGlobalNtpOffsetInMilliseconds = offset;
                Console.WriteLine($"Default Global NTP Offset: {defaultGlobalNtpOffsetInMilliseconds} {ntpServer}");
            }
            catch (Exception)
            {
                defaultGlobalNtpOffsetInMilliseconds = 0;
            }
        }

        public static int GetGlobalNtpOffsetInMilliseconds()
        {
            return defaultGlobalNtpOffsetInMilliseconds;
        }

        private static uint SwapEndianness(ulong x)
        {
            return (uint)(((x & 0x000000ff) << 24) + ((x & 0x0000ff00) << 8) + ((x & 0x00ff0000) >> 8) + ((x & 0xff000000) >> 24));
        }

        public static void GetTimesFromOffsetMilliseconds(
            int ntpOffsetLocalToServerMilliseconds,
            out long localTimeNoneUTCInMilliseconds, 
            out long localTimeInMillisecondsUTC,
            out long ntpTimeInMillisecondsUTC)
        {
            long ticks = DateTime.UtcNow.Ticks;
            long ticksLocal = DateTime.Now.Ticks;
            localTimeNoneUTCInMilliseconds = ticksLocal / 10000;
            localTimeInMillisecondsUTC = ticks / 10000;
            ntpTimeInMillisecondsUTC = (ticks + ntpOffsetLocalToServerMilliseconds * 10000) / 10000;

        }
        public static void GetTimesFromOffsetMilliseconds(
           int ntpOffsetLocalToServerMilliseconds,
            out long localTimeNoneUTCInMilliseconds,
            out long localTimeInMillisecondsUTC,
            out long ntpTimeInMillisecondsUTC,
            out string localTimeNoneUTCInMillisecondsStr,
            out string localTimeInMillisecondsUTCStr,
            out string ntpTimeInMillisecondsUTCStr,
           string dateFormat="yyyy-MM-dd ")
        {

            GetTimesFromOffsetMilliseconds(
                ntpOffsetLocalToServerMilliseconds,
                out localTimeNoneUTCInMilliseconds,
                out localTimeInMillisecondsUTC,
                out ntpTimeInMillisecondsUTC
                );
            localTimeNoneUTCInMillisecondsStr = new DateTime(localTimeNoneUTCInMilliseconds * 10000).ToString(dateFormat);
            localTimeInMillisecondsUTCStr = new DateTime(localTimeInMillisecondsUTC * 10000).ToString(dateFormat);
            ntpTimeInMillisecondsUTCStr = new DateTime(ntpTimeInMillisecondsUTC * 10000).ToString(dateFormat);


        }
    } 
}


    

    
    
  

    
        
        
            
            
        
        
        
        
 
        
        
        
        
        
        
        
        
        
        
        

    
        
   
        
   
        
        
    
     


    
    



        

        
                
        
        
        
        



        
    
    
            
    
        
        
                    
                    
                    
                    
                    
       
       
       
    
        
            
            


                








        
        
        
        
        
        
        
    
        
    




















































    
    
            
            
        
    


            
        
        
                


            
            
        
    

        


   

        
 

 

        
    



        
        























    
        


      
    


        
            
            
            