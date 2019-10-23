using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace WindowsProcessMonitor
{
    public class ProcessMonitor
    {
        public static void CollectStats(Object source, ElapsedEventArgs e)
        {
            try
            {
                var remoteById = Process.GetProcessById(Runner.PId);

                using (PerformanceCounter pcProcess = new PerformanceCounter("Process", "% Processor Time", remoteById.ProcessName))
                {
                    pcProcess.NextValue();
                    System.Threading.Thread.Sleep(1000);
                    //Console.WriteLine("Process:{0} CPU% {1}", remoteById.ProcessName, pcProcess.NextValue());
                    Runner.processMonitorStats.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + remoteById.Id.ToString(CultureInfo.InvariantCulture) + ", " + remoteById.ProcessName + ", " +
                    remoteById.HandleCount + ", " + remoteById.Threads.Count + ", " + remoteById.WorkingSet64 + ", " +
                    remoteById.PrivateMemorySize64 + ", " + remoteById.PagedMemorySize64 + ", " + remoteById.PeakWorkingSet64 + ", " + pcProcess.NextValue());
                }

                

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
