using System;

namespace NWN
{
    public static class Utils
    {
        public static void LogException(Exception e)
        {
            Console.WriteLine(e.Message);
            NWScript.SendMessageToAllDMs(e.Message);
            NWScript.WriteTimestampedLogEntry(e.Message);
        }
    }
}
