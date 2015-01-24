using System;
using System.IO;

namespace DeBouncer
{
    public static class Logger
    {
        public static string LogFileName { get; set; }
        private static Object _LockObject = new Object();
        public static bool DisableLogging { get; set; }
        
        public static void LogMessage(string message)
        {
            lock (_LockObject)
            {
                if (DisableLogging) return;

                using (var sw = new StreamWriter(LogFileName, true))
                {
                    sw.WriteLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + " " + message);
#if (DEBUG)
                    Console.WriteLine(message);
#endif
                }
            }
        }
    }
}
