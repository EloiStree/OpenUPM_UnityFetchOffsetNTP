# 2024_07_07_UnityFetchOffsetNTP
Just a tool to fetch the offset from a NTP server in Unity3D

![image](https://github.com/user-attachments/assets/26793c72-a219-49d3-ac59-d62b0adc14be)


If you need to check with Python:
``` py

import ntplib
from time import ctime, time, sleep

def check_ntp_server(server):
    client = ntplib.NTPClient()
    try:
        response = client.request(server, version=3)
        ntp_time = response.tx_time
        local_time = time()
        time_difference = (local_time - ntp_time) * 1000  # Convert to milliseconds
        print(f"Server {server} is reachable.")
        print(f"NTP Time: {ctime(ntp_time)}")
        print(f"Local Time: {ctime(local_time)}")
        print(f"Time Difference: {time_difference:.2f} ms")
    except Exception as e:
        print(f"Failed to reach NTP server {server}. Error: {e}")

if __name__ == "__main__":
    ntp_server = '192.168.1.118'
    ntp_server = '193.150.14.47'
    ntp_server = 'raspberrypi5'
    ntp_server = 'time.google.com'
    
    while True:
        check_ntp_server(ntp_server)
        sleep(1)

```
