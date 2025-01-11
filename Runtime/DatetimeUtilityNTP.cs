using System;

public class DatetimeUtilityNTP {


    public static void GetTimeNowTicksUtcLocal(out long tickTime)
    {
        tickTime = System.DateTime.UtcNow.Ticks;
    }

    public static void GetTimeNowTicksUtc(long offsetMilliseconds, out long tickTime)
    {
        GetTimeNowTicksUtcLocal(out tickTime);
        tickTime += (offsetMilliseconds * System.TimeSpan.TicksPerMillisecond);
    }
    public static void GetTimeNowTicksUtcSeconds(long offsetMilliseconds, out int seconds) { 
        GetTimeNowTicksUtc(offsetMilliseconds, out long tickTime);
        seconds = (int)(tickTime / System.TimeSpan.TicksPerSecond);
    }
    public static void GetTimeNowTicksUtcSeconds(long offsetMilliseconds, out double seconds)
    {
        GetTimeNowTicksUtc(offsetMilliseconds, out long tickTime);
        seconds = ((double)tickTime / (double)System.TimeSpan.TicksPerSecond);
    }


}
