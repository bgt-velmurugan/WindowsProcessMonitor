using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsProcessMonitor
{
    public static class PerformanceMonitorLibrary
    {
        public class PerfCount
        {
            public Counters CounterType { get; set; }
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public PerformanceCounter Counters { get; set; }
        }

        public class PerformanceCounterFactory
        {
            private readonly Dictionary<string, List<PerfCount>> _counters = new Dictionary<string, List<PerfCount>>();
            public Dictionary<string, List<PerfCount>> Items { get { return _counters; } }
            private static PerformanceCounterFactory _instance;

            public static PerformanceCounterFactory Instance
            {
                get { return _instance ?? (_instance = new PerformanceCounterFactory()); }
            }

            public void CollectPerformance(string machine, Counters counter, string categoryName, string counterName, string instanceName)
            {
                try
                {
                    if (_counters.ContainsKey(machine))
                    {
                        _counters.Where(key => key.Key == machine).Select(s => s).First().Value.Add(new PerfCount { CounterType = counter, Counters = new PerformanceCounter(categoryName, counterName, instanceName, machine) });
                    }
                    else
                    {
                        _counters.Add(machine, new List<PerfCount> { new PerfCount { CounterType = counter, Counters = new PerformanceCounter(categoryName, counterName, instanceName, machine) } });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception while capturning Performance Counter for Machine - " + machine);
                    Console.WriteLine(ex);
                }
            }

            public static IEnumerable<T> GetValues<T>()
            {
                return Enum.GetValues(typeof(T)).Cast<T>();
            }

            public void Initiate(string machine)
            {
                foreach (var counter in GetValues<Counters>())
                {
                    switch (counter)
                    {
                        case Counters.Memory:
                            CollectPerformance(machine, counter, "Memory", "% Committed Bytes In Use", String.Empty);
                            break;
                        case Counters.Process:
                            CollectPerformance(machine, counter, "Process", "% Processor Time", "_Total");
                            break;
                        case Counters.Processor:
                            CollectPerformance(machine, counter, "Processor", "% Processor Time", "_Total");
                            break;
                        case Counters.ProcessorInfo:
                            CollectPerformance(machine, counter, "Processor Information", "% Processor Time", "_Total");
                            break;
                    }
                }
            }

            public void DisposeCounters()
            {
                try
                {
                    foreach (var item in Instance.Items)
                    {
                        foreach (var counter in item.Value)
                        {
                            counter.Counters.Dispose();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception while disposing the Performance Counters");
                    Console.WriteLine(ex);
                }
                finally
                {
                    PerformanceCounter.CloseSharedResources();
                }
            }
        }

        public enum Counters
        {
            Memory = 0,
            Process = 1,
            Processor = 2,
            ProcessorInfo = 3
        }
    }
}
