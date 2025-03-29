

using System;
using System.Net;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Eloi.IID
{
    public class DNSToIPV4Mono : MonoBehaviour{

    
        public string m_dnsAddressReceived;
        public string m_ipv4AddressFound;

        public UnityEvent<string> m_onIpv4Found;
        public UnityEvent<string> m_onRequestToParseNotSupportedAddress;

        public bool m_didNotSupportDnsAddress;
        public string m_errorMessage;
        public bool m_useDebugLogForError = true;

        public bool m_useOfLocalDetected;
        public bool m_isDeviceSupportUseOfLocal;

        public void PushIn(string dnsAddress) {
            m_dnsAddressReceived = dnsAddress;
            m_useOfLocalDetected = dnsAddress.Contains(".local");
            IPAddress[] addresses = null;
            string[] split = dnsAddress.Split('.');
            bool isIpv4 = split.Length >= 4
                && int.TryParse(split[0], out int _)
                && int.TryParse(split[1], out int _)
                && int.TryParse(split[2], out int _)
                && int.TryParse(split[3], out int _);
            if (isIpv4) {

                m_ipv4AddressFound = dnsAddress;
                m_onIpv4Found.Invoke(m_ipv4AddressFound);
                return;
            }
            try
            {
                addresses = System.Net.Dns.GetHostEntry(dnsAddress).AddressList;
                UnityEngine.Debug.Log($"NTP Fetch: {dnsAddress} {string.Join(",", addresses.Select(a => a.ToString()))}");
                if (addresses != null && addresses.Length > 0)
                {
                    m_ipv4AddressFound = addresses[0].ToString();
                    if (dnsAddress.Contains(".local"))
                    {
                        m_isDeviceSupportUseOfLocal = true;
                    }
                
                }
                else
                {
                    m_didNotSupportDnsAddress = true;
                    m_errorMessage = "No address found";
                    m_onRequestToParseNotSupportedAddress.Invoke(m_dnsAddressReceived);
                    return;
                }
            }
            catch (Exception e)
            {
                m_didNotSupportDnsAddress = true;
                m_errorMessage = e.Message;
                if (m_useDebugLogForError)
                    UnityEngine.Debug.LogWarning("Error resolving IP address: " + e.Message + "\n" + e.StackTrace);
                m_onRequestToParseNotSupportedAddress.Invoke(m_dnsAddressReceived);
                return;
            }
        }

    }
}


    

    
    
  

    
        
        
            
            
        
        
        
        
 
        
        
        
        
        
        
        
        
        
        
        

    
        
   
        
   
        
        
    
     


    
    



        

        
                
        
        
        
        



        
    
    
            
    
        
        
                    
                    
                    
                    
                    
       
       
       
    
        
            
            


                








        
        
        
        
        
        
        
    
        
    




















































    
    
            
            
        
    


            
        
        
                


            
            
        
    

        


   

        
 

 

        
    



        
        























    
        


      
    


        
            
            
            