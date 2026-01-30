using System;

namespace CsvToExamples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== CsvTo 库使用示例 ===\n");

            while (true)
            {
                Console.WriteLine("请选择示例:");
                Console.WriteLine("1. 基本用法 - DataTable 转换");
                Console.WriteLine("2. 基本用法 - 集合转换");
                Console.WriteLine("3. 泛型转换示例");
                Console.WriteLine("4. 多语言编码示例");
                Console.WriteLine("5. 自定义分隔符示例");
                Console.WriteLine("6. 反向读取示例");
                Console.WriteLine("7. Stream 输入示例");
                Console.WriteLine("8. 高级用法 - 复杂类型转换");
                Console.WriteLine("9. 错误处理示例");
                Console.WriteLine("0. 退出");
                Console.Write("\n请输入选项 (0-9): ");

                var input = Console.ReadLine();
                Console.WriteLine();

                switch (input)
                {
                    case "1":
                        BasicDataTableExample.Run();
                        break;
                    case "2":
                        BasicCollectionExample.Run();
                        break;
                    case "3":
                        GenericConverterExample.Run();
                        break;
                    case "4":
                        EncodingExample.Run();
                        break;
                    case "5":
                        CustomDelimiterExample.Run();
                        break;
                    case "6":
                        ReverseReadingExample.Run();
                        break;
                    case "7":
                        StreamInputExample.Run();
                        break;
                    case "8":
                        AdvancedTypesExample.Run();
                        break;
                    case "9":
                        ErrorHandlingExample.Run();
                        break;
                    case "0":
                        Console.WriteLine("感谢使用！再见！");
                        return;
                    default:
                        Console.WriteLine("无效选项，请重试。\n");
                        break;
                }

                Console.WriteLine("\n按任意键继续...");
                Console.ReadKey();
                Console.Clear();
            }
        }
    }
}
