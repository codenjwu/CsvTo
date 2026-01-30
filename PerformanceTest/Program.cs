using BenchmarkDotNet.Running;

namespace PerformanceTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<CsvConverterBenchmarks>();
        }
    }
}
