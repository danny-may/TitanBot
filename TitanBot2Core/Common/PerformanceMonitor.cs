using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot2.Common
{
    public static class PerformanceMonitor
    {
        static PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        static PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes");

        public static string getCurrentCPUUsage()
            => cpuCounter.NextValue() + "%";

        public static string getAvailableRAM()
            => ramCounter.NextValue() + "MB";
    }
}
