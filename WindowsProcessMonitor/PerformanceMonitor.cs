using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace WindowsProcessMonitor
{
    public class PerformanceMonitor
    {
        public static void CollectPerformanceCounterSets(Object source, ElapsedEventArgs e)
        {
            try
            {
                foreach (var item in PerformanceMonitorLibrary.PerformanceCounterFactory.Instance.Items)
                {
                    foreach (var counter in item.Value)
                    {
                        Runner.performanceMonitorStats.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + counter.Counters.MachineName
                            + "," + counter.CounterType.ToString() + "," + counter.Counters.NextValue() + ","
                            + counter.Counters.RawValue);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception while collecting Performance Monitor counters");
                Console.WriteLine(ex);
            }
        }
    }
}
