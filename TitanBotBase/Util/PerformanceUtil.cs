using System.Diagnostics;

namespace TitanBotBase.Util
{
    public static class PerformanceUtil
    {
        static PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        static PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes");

        public static string getCurrentCPUUsage()
            => cpuCounter.NextValue() + "%";

        public static string getAvailableRAM()
            => ramCounter.NextValue() + "MB";
    }
}
