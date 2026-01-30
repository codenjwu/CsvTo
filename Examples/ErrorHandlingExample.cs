using System;
using System.IO;

namespace CsvToExamples
{
    public static class ErrorHandlingExample
    {
        public static void Run()
        {
            Console.WriteLine("=== 示例 9: 错误处理示例 ===\n");

            // 示例 1: 文件不存在
            Console.WriteLine("1. 处理文件不存在的情况:");
            try
            {
                var converter = new CsvTo.CsvConverter("nonexistent.csv");
                var data = converter.ToDataTable();
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"   ✓ 捕获异常: 文件未找到");
                Console.WriteLine($"   消息: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   其他异常: {ex.Message}");
            }

            // 示例 2: 空文件处理
            Console.WriteLine("\n2. 处理空文件:");
            string emptyFile = "empty.csv";
            File.WriteAllText(emptyFile, string.Empty);
            
            try
            {
                var converter = new CsvTo.CsvConverter(emptyFile);
                var data = converter.ToDataTable();
                Console.WriteLine($"   ✓ 空文件处理成功，返回 {data.Rows.Count} 行");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   异常: {ex.Message}");
            }

            // 示例 3: 格式错误的 CSV
            Console.WriteLine("\n3. 处理格式不一致的 CSV:");
            string malformedFile = "malformed.csv";
            File.WriteAllLines(malformedFile, new[]
            {
                "Name,Age,City",
                "Alice,25,New York",
                "Bob,30",  // 缺少一列
                "Charlie,28,Chicago,Extra"  // 多了一列
            });

            try
            {
                var converter = new CsvTo.CsvConverter(malformedFile);
                var data = converter.ToCollection();
                
                Console.WriteLine("   读取结果:");
                foreach (var row in data)
                {
                    Console.WriteLine($"   [{string.Join(", ", row)}] (列数: {row.Length})");
                }
                Console.WriteLine("   注意: CsvTo 会按实际列数读取，不会抛出异常");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   异常: {ex.Message}");
            }

            // 示例 4: 类型转换错误
            Console.WriteLine("\n4. 处理类型转换错误:");
            string typeErrorFile = "type_error.csv";
            File.WriteAllLines(typeErrorFile, new[]
            {
                "Id,Name,Age",
                "1,Alice,25",
                "2,Bob,invalid_age",  // 无效的年龄值
                "3,Charlie,30"
            });

            try
            {
                var converter = new CsvTo.CsvConverter<PersonWithValidation>(typeErrorFile);
                var people = converter.ToCollection();
                
                foreach (var person in people)
                {
                    Console.WriteLine($"   {person}");
                }
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"   ✓ 捕获类型转换异常");
                Console.WriteLine($"   消息: {ex.Message}");
                Console.WriteLine("   建议: 在转换前验证数据，或使用可空类型");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   异常: {ex.GetType().Name} - {ex.Message}");
            }

            // 示例 5: 最佳实践
            Console.WriteLine("\n5. 错误处理最佳实践:");
            Console.WriteLine("   ✓ 使用 try-catch 捕获可能的异常");
            Console.WriteLine("   ✓ 验证文件是否存在: File.Exists(path)");
            Console.WriteLine("   ✓ 使用可空类型处理可能为空的数据");
            Console.WriteLine("   ✓ 提供有意义的错误消息");
            Console.WriteLine("   ✓ 记录日志以便调试");

            // 清理测试文件
            CleanupFiles(emptyFile, malformedFile, typeErrorFile);
        }

        private static void CleanupFiles(params string[] files)
        {
            foreach (var file in files)
            {
                if (File.Exists(file))
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch
                    {
                        // 忽略清理错误
                    }
                }
            }
        }
    }

    public class PersonWithValidation
    {
        [CsvTo.CsvColumn("Id")]
        public int Id { get; set; }

        [CsvTo.CsvColumn("Name")]
        public string Name { get; set; } = string.Empty;

        [CsvTo.CsvColumn("Age")]
        public int Age { get; set; }

        public override string ToString()
        {
            return $"[{Id}] {Name}, {Age}岁";
        }
    }
}
