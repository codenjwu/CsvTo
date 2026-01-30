using System;
using System.IO;
using System.Linq;
using System.Text;

namespace CsvToExamples
{
    public static class StreamInputExample
    {
        public static void Run()
        {
            Console.WriteLine("=== 示例 7: Stream 输入示例 ===\n");

            try
            {
                // 示例 1: 从内存流读取
                Console.WriteLine("1. 从内存流读取:");
                string csvContent = @"Name,Age,City
Alice,25,New York
Bob,30,Los Angeles
Charlie,28,Chicago";

                using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent)))
                {
                    var converter = new CsvTo.CsvConverter(memoryStream);
                    var data = converter.ToDataTable();
                    
                    Console.WriteLine($"   读取 {data.Rows.Count} 行数据");
                    foreach (System.Data.DataRow row in data.Rows)
                    {
                        Console.WriteLine($"   {row[0]}, {row[1]}, {row[2]}");
                    }
                }

                // 示例 2: 从网络流读取（模拟）
                Console.WriteLine("\n2. 从文件流读取:");
                string tempFile = "temp_stream_test.csv";
                File.WriteAllText(tempFile, csvContent);

                using (var fileStream = File.OpenRead(tempFile))
                {
                    var converter = new CsvTo.CsvConverter(fileStream);
                    var collection = converter.ToCollection().ToList();
                    
                    Console.WriteLine($"   读取 {collection.Count} 行");
                    foreach (var row in collection)
                    {
                        Console.WriteLine($"   [{string.Join(", ", row)}]");
                    }
                }

                if (File.Exists(tempFile))
                    File.Delete(tempFile);

                // 示例 3: Stream 的优势
                Console.WriteLine("\n3. Stream 输入的优势:");
                Console.WriteLine("   - 可以从任何 Stream 源读取（内存、网络、压缩流等）");
                Console.WriteLine("   - 不需要先保存到临时文件");
                Console.WriteLine("   - 适合处理动态生成的 CSV 数据");
                Console.WriteLine("   - 可以与其他流操作（加密、压缩）组合使用");

                // 示例 4: 带编码的 Stream
                Console.WriteLine("\n4. 带编码的 Stream 示例:");
                string chineseContent = "姓名,年龄,城市\n张三,25,北京\n李四,30,上海";
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(chineseContent)))
                {
                    var converter = new CsvTo.CsvConverter(stream, encoding: Encoding.UTF8);
                    var data = converter.ToDataTable();
                    Console.WriteLine($"   成功读取中文数据，共 {data.Rows.Count} 行");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"错误: {ex.Message}");
            }
        }
    }
}
