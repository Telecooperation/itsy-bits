using System.Runtime.InteropServices;

namespace OptiTrack
{
    public class HighPerformanceTimer
    {
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);

        private long start;
        private readonly long freq;

        public HighPerformanceTimer()
        {
            start = 0;
            QueryPerformanceFrequency(out freq);
        }

        public void Start()
        {
            QueryPerformanceCounter(out start);
        }

        public double Stop()
        {
            long stop;
            QueryPerformanceCounter(out stop);
            return (stop - start) / (double)freq;
        }
    }
}
