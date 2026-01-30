using System;
using System.Text;

namespace CsvToExamples
{
    public static class EncodingExample
    {
        public static void Run()
        {
            Console.WriteLine("=== 示例 4: 多语言编码示例 ===\n");

            // 创建不同编码的 CSV 文件
            string utf8File = "chinese_utf8.csv";
            string utf16File = "japanese_utf16.csv";

            CreateChineseCsv(utf8File);
            CreateJapaneseCsv(utf16File);

            try
            {
                // 示例 1: UTF-8 中文文件
                Console.WriteLine("1. 读取 UTF-8 中文文件:");
                var converter1 = new CsvTo.CsvConverter(utf8File, encoding: Encoding.UTF8);
                var data1 = converter1.ToDataTable();
                
                Console.WriteLine($"   行数: {data1.Rows.Count}");
                foreach (System.Data.DataRow row in data1.Rows)
                {
                    Console.WriteLine($"   {row[0]} - {row[1]}");
                }

                // 示例 2: UTF-16 日文文件
                Console.WriteLine("\n2. 读取 UTF-16 日文文件:");
                var converter2 = new CsvTo.CsvConverter(utf16File, encoding: Encoding.Unicode);
                var data2 = converter2.ToDataTable();
                
                Console.WriteLine($"   行数: {data2.Rows.Count}");
                foreach (System.Data.DataRow row in data2.Rows)
                {
                    Console.WriteLine($"   {row[0]} - {row[1]}");
                }

                // 示例 3: 默认编码（UTF-8）
                Console.WriteLine("\n3. 使用默认编码 (UTF-8):");
                var converter3 = new CsvTo.CsvConverter(utf8File);  // 默认使用 UTF-8
                var data3 = converter3.ToDataTable();
                Console.WriteLine($"   成功读取 {data3.Rows.Count} 行数据");

                // 示例 4: GB2312 编码（如果需要）
                Console.WriteLine("\n4. 支持的其他编码:");
                Console.WriteLine("   - UTF-8 (默认)");
                Console.WriteLine("   - UTF-16 (Unicode)");
                Console.WriteLine("   - GB2312 (简体中文)");
                Console.WriteLine("   - Big5 (繁体中文)");
                Console.WriteLine("   - Shift-JIS (日文)");
                Console.WriteLine("   - 等等...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"错误: {ex.Message}");
            }
        }

        private static void CreateChineseCsv(string filename)
        {
            var lines = new[]
            {
                "编号,名称",
                "1,北京",
                "2,上海",
                "3,广州",
                "4,深圳"
            };
            System.IO.File.WriteAllLines(filename, lines, Encoding.UTF8);
        }

        private static void CreateJapaneseCsv(string filename)
        {
            var lines = new[]
            {
                "番号,都市名",
                "1,東京",
                "2,大阪",
                "3,京都",
                "4,横浜"
            };
            System.IO.File.WriteAllLines(filename, lines, Encoding.Unicode);
        }
    }
}
