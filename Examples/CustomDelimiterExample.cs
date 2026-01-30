using System;
using System.Linq;

namespace CsvToExamples
{
    public static class CustomDelimiterExample
    {
        public static void Run()
        {
            Console.WriteLine("=== 示例 5: 自定义分隔符示例 ===\n");

            // 创建使用不同分隔符的文件
            string semicolonFile = "data_semicolon.csv";
            string tabFile = "data_tab.tsv";
            string pipeFile = "data_pipe.txt";

            CreateSemicolonCsv(semicolonFile);
            CreateTabCsv(tabFile);
            CreatePipeCsv(pipeFile);

            try
            {
                // 示例 1: 分号分隔
                Console.WriteLine("1. 分号分隔的文件 (;):");
                var converter1 = new CsvTo.CsvConverter(semicolonFile, delimiter: ";");
                var data1 = converter1.ToCollection().ToList();
                DisplayData(data1);

                // 示例 2：Tab 分隔
                Console.WriteLine("\n2. Tab 分隔的文件 (\\t):");
                var converter2 = new CsvTo.CsvConverter(tabFile, delimiter: "\t");
                var data2 = converter2.ToCollection().ToList();
                DisplayData(data2);

                // 示例 3：管道符分隔
                Console.WriteLine("\n3. 管道符分隔的文件 (|):");
                var converter3 = new CsvTo.CsvConverter(pipeFile, delimiter: "|");
                var data3 = converter3.ToCollection().ToList();
                DisplayData(data3);

                // 示例 4: 自定义转义字符
                Console.WriteLine("\n4. 自定义转义字符:");
                Console.WriteLine("   默认转义字符是双引号 (\")");
                Console.WriteLine("   可以通过 escape 参数自定义");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"错误: {ex.Message}");
            }
        }

        private static void DisplayData(System.Collections.Generic.List<string[]> data)
        {
            foreach (var row in data)
            {
                Console.WriteLine($"   [{string.Join(" | ", row)}]");
            }
        }

        private static void CreateSemicolonCsv(string filename)
        {
            var lines = new[]
            {
                "Product;Price;Quantity",
                "Apple;1.50;100",
                "Banana;0.80;150",
                "Orange;2.00;80"
            };
            System.IO.File.WriteAllLines(filename, lines);
        }

        private static void CreateTabCsv(string filename)
        {
            var lines = new[]
            {
                "Name\tScore\tGrade",
                "Alice\t95\tA",
                "Bob\t87\tB",
                "Charlie\t92\tA"
            };
            System.IO.File.WriteAllLines(filename, lines);
        }

        private static void CreatePipeCsv(string filename)
        {
            var lines = new[]
            {
                "ID|Username|Email",
                "1|user001|user001@example.com",
                "2|user002|user002@example.com",
                "3|user003|user003@example.com"
            };
            System.IO.File.WriteAllLines(filename, lines);
        }
    }
}
