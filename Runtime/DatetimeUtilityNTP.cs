public class DatetimeUtilityNTP {


    public static void GetTimeNowUtcLocal(out long tickTime)
    {
        tickTime = System.DateTime.UtcNow.Ticks;
    }

    public static void GetTimeNowUtcTicks(long offset, out long tickTime)
    {
        GetTimeNowUtcLocal(out tickTime);
        tickTime += offset;
    }
    public static void GetTimeNowUtcSeconds(long offset, out int seconds) { 
        GetTimeNowUtcTicks(offset, out long tickTime);
        seconds = (int)(tickTime / System.TimeSpan.TicksPerSecond);
    }
    public static void GetTimeNowUtcSeconds(long offset, out double seconds)
    {
        GetTimeNowUtcTicks(offset, out long tickTime);
        seconds = ((double)tickTime / (double)System.TimeSpan.TicksPerSecond);
    }


}
