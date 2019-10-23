
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Timers;

namespace WindowsProcessMonitor
{
    public class Runner
    {
        public static int PId;
        public static StreamWriter processMonitorStats;
        public static StreamWriter performanceMonitorStats;

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please enter the process id.");
                Console.WriteLine("Usage: WindowsProcessMonitor.exe PID");
                return;
            }

            PId = Convert.ToInt32(args[0]);
            PerformanceMonitorLibrary.PerformanceCounterFactory.Instance.Initiate("localhost");

            using (performanceMonitorStats = new StreamWriter("PerformanceMonitorResults.csv"))
            {
                performanceMonitorStats.AutoFlush = true;
                performanceMonitorStats.WriteLine("SliceTime, MachineName, InstanceName, CounterValue, RawValue");

                using (processMonitorStats = new StreamWriter("ProcessMonitorResults.csv"))
                {
                    processMonitorStats.AutoFlush = true;
                    processMonitorStats.WriteLine("SliceTime, ProcessId, ProcessName, HandleCount, ThreadsCount, WorkingSet, PrivateMemorySize, PagedMemorySize, PeakWorkingSet, CPU %");
                    InitTimer();
                    Console.Read();
                }
            }
        }

        public static void InitTimer()
        {
            var processTimer = new Timer(10000);
            processTimer.Elapsed += (source, e) => ProcessMonitor.CollectStats(source, e);
            processTimer.Elapsed += (source, e) => PerformanceMonitor.CollectPerformanceCounterSets(source, e);
            processTimer.Enabled = true;
        }
    }
}
