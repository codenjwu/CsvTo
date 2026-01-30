using System;
using System.Collections.Generic;
using System.Linq;

namespace CsvToExamples
{
    public static class BasicCollectionExample
    {
        public static void Run()
        {
            Console.WriteLine("=== 示例 2: 基本用法 - 集合转换 ===\n");

            string csvFile = "example2.csv";
            CreateSampleCsv(csvFile);

            try
            {
                // 创建转换器
                var converter = new CsvTo.CsvConverter(csvFile);

                // 转换为集合
                IEnumerable<string[]> collection = converter.ToCollection();

                // 显示结果
                Console.WriteLine($"CSV 文件: {csvFile}\n");

                int rowNumber = 0;
                foreach (var row in collection)
                {
                    Console.WriteLine($"行 {rowNumber++}: [{string.Join(", ", row)}]");
                }

                // 演示 LINQ 查询
                Console.WriteLine("\n=== LINQ 查询示例 ===");
                var rows = collection.ToList();
                
                // 跳过表头，获取数据行
                var dataRows = rows.Skip(1).ToList();
                Console.WriteLine($"\n总共有 {dataRows.Count} 条数据记录");

                // 查找年龄大于 25 的记录
                Console.WriteLine("\n年龄大于 25 的记录:");
                foreach (var row in dataRows)
                {
                    if (int.TryParse(row[2], out int age) && age > 25)
                    {
                        Console.WriteLine($"  {row[1]}, {age} 岁");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"错误: {ex.Message}");
            }
        }

        private static void CreateSampleCsv(string filename)
        {
            var lines = new[]
            {
                "Id,Name,Age,Department",
                "1,Alice,23,Engineering",
                "2,Bob,27,Marketing",
                "3,Charlie,31,Sales",
                "4,Diana,24,Engineering",
                "5,Eve,29,HR"
            };
            System.IO.File.WriteAllLines(filename, lines);
        }
    }
}
