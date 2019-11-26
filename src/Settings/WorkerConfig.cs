namespace HostedService.Settings
{
    public class WorkerConfig
    {
        public int MinServiceDelay { get; set; }

        public int MaxServiceDelay { get; set; }

        public bool StressTest { get; set; }

        public int StressTestDelay { get; set; }

        public int StressTestCores { get; set; }

    }
}