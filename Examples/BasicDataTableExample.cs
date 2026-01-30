using System;
using System.Data;

namespace CsvToExamples
{
    public static class BasicDataTableExample
    {
        public static void Run()
        {
            Console.WriteLine("=== 示例 1: 基本用法 - DataTable 转换 ===\n");

            // 创建 CSV 文件
            string csvFile = "example1.csv";
            CreateSampleCsv(csvFile);

            try
            {
                // 创建转换器
                var converter = new CsvTo.CsvConverter(csvFile);

                // 转换为 DataTable
                DataTable dataTable = converter.ToDataTable();

                // 显示结果
                Console.WriteLine($"CSV 文件: {csvFile}");
                Console.WriteLine($"行数: {dataTable.Rows.Count}");
                Console.WriteLine($"列数: {dataTable.Columns.Count}");
                Console.WriteLine("\n表头:");
                foreach (DataColumn col in dataTable.Columns)
                {
                    Console.Write($"{col.ColumnName}\t");
                }
                Console.WriteLine("\n\n数据:");

                foreach (DataRow row in dataTable.Rows)
                {
                    foreach (var item in row.ItemArray)
                    {
                        Console.Write($"{item}\t");
                    }
                    Console.WriteLine();
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
                "Id,Name,Age,Email",
                "1,张三,25,zhangsan@example.com",
                "2,李四,30,lisi@example.com",
                "3,王五,28,wangwu@example.com"
            };
            System.IO.File.WriteAllLines(filename, lines);
        }
    }
}
