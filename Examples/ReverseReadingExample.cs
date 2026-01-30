using System;
using System.Linq;

namespace CsvToExamples
{
    public static class ReverseReadingExample
    {
        public static void Run()
        {
            Console.WriteLine("=== 示例 6: 反向读取示例 ===\n");

            string csvFile = "log_data.csv";
            CreateLogCsv(csvFile);

            try
            {
                Console.WriteLine("正向读取:");
                var forwardConverter = new CsvTo.CsvConverter(csvFile);
                var forwardData = forwardConverter.ToCollection().ToList();
                
                Console.WriteLine("前 3 行:");
                foreach (var row in forwardData.Take(4))  // 包括表头
                {
                    Console.WriteLine($"  {string.Join(", ", row)}");
                }

                Console.WriteLine("\n反向读取:");
                var reverseConverter = new CsvTo.CsvReverseConverter(csvFile);
                var reverseData = reverseConverter.ToCollection().ToList();
                
                Console.WriteLine("最后 3 行 (从文件末尾开始):");
                foreach (var row in reverseData.Take(3))
                {
                    Console.WriteLine($"  {string.Join(", ", row)}");
                }

                Console.WriteLine("\n用途示例:");
                Console.WriteLine("1. 读取日志文件的最新记录");
                Console.WriteLine("2. 获取最近的交易记录");
                Console.WriteLine("3. 快速查看文件末尾数据而不需要加载整个文件");

                // 泛型反向转换示例
                Console.WriteLine("\n泛型反向转换:");
                var genericReverseConverter = new CsvTo.CsvReverseConverter<LogEntry>(csvFile);
                var logEntries = genericReverseConverter.ToCollection().Take(3).ToList();
                
                Console.WriteLine("最近 3 条日志:");
                foreach (var entry in logEntries)
                {
                    Console.WriteLine($"  {entry}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"错误: {ex.Message}");
            }
        }

        private static void CreateLogCsv(string filename)
        {
            var lines = new[]
            {
                "Timestamp,Level,Message",
                "2026-01-29 08:00:00,INFO,Application started",
                "2026-01-29 08:05:23,INFO,User login: admin",
                "2026-01-29 08:15:45,WARNING,High memory usage",
                "2026-01-29 08:30:12,INFO,Database backup completed",
                "2026-01-29 08:45:33,ERROR,Connection timeout",
                "2026-01-29 09:00:00,INFO,Scheduled task executed",
                "2026-01-29 09:15:27,INFO,Cache cleared",
                "2026-01-29 09:30:50,WARNING,Disk space low",
                "2026-01-29 09:45:15,INFO,Report generated"
            };
            System.IO.File.WriteAllLines(filename, lines);
        }
    }

    public class LogEntry
    {
        [CsvTo.CsvColumn("Timestamp")]
        public string Timestamp { get; set; } = string.Empty;

        [CsvTo.CsvColumn("Level")]
        public string Level { get; set; } = string.Empty;

        [CsvTo.CsvColumn("Message")]
        public string Message { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"[{Timestamp}] {Level}: {Message}";
        }
    }
}
