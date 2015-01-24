using System;
using System.IO;
using System.ServiceProcess;

namespace DeBouncer
{
    public class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
#if (!DEBUG)
            var servicesToRun = new ServiceBase[] 
            { 
                new WatchDirector() 
            };
            ServiceBase.Run(servicesToRun);
#else
            var watchDirector = new WatchDirector();
            watchDirector.Start();
            Console.WriteLine("Press any key to quit.");
            Console.ReadLine();
#endif
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            using (var sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "CriticalError.txt", true))
            {
                var exp = e.ExceptionObject as Exception;
                if (exp != null)
                {
                    sw.WriteLine(DateTime.Now.ToShortTimeString() + exp.Message);
                    sw.WriteLine(DateTime.Now.ToShortTimeString() + exp.Source);
                    sw.WriteLine(DateTime.Now.ToShortTimeString() + exp.StackTrace);
                }
                else
                {
                    sw.WriteLine(DateTime.Now.ToShortTimeString() + " Global unhandled with no exception object");
                }
            }
        }
    }
}
