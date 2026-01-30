using System;
using System.Collections.Generic;
using System.Linq;

namespace CsvToExamples
{
    public static class GenericConverterExample
    {
        public static void Run()
        {
            Console.WriteLine("=== 示例 3: 泛型转换示例 ===\n");

            string csvFile = "example3.csv";
            CreateSampleCsv(csvFile);

            try
            {
                // 使用泛型转换器
                var converter = new CsvTo.CsvConverter<Person>(csvFile);

                // 转换为强类型集合
                IEnumerable<Person> people = converter.ToCollection();

                // 显示结果
                Console.WriteLine("强类型对象列表:\n");
                foreach (var person in people)
                {
                    Console.WriteLine(person);
                }

                // LINQ 查询
                var peopleList = people.ToList();
                Console.WriteLine($"\n总人数: {peopleList.Count}");
                
                var averageAge = peopleList.Average(p => p.Age);
                Console.WriteLine($"平均年龄: {averageAge:F1}");

                var engineeringCount = peopleList.Count(p => p.Department == "Engineering");
                Console.WriteLine($"工程部门人数: {engineeringCount}");

                Console.WriteLine("\n30岁以上的员工:");
                foreach (var person in peopleList.Where(p => p.Age >= 30))
                {
                    Console.WriteLine($"  - {person.Name}, {person.Department}");
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
                "1,张三,28,Engineering",
                "2,李四,35,Marketing",
                "3,王五,31,Engineering",
                "4,赵六,26,Sales",
                "5,钱七,40,HR"
            };
            System.IO.File.WriteAllLines(filename, lines);
        }
    }

    public class Person
    {
        [CsvTo.CsvColumn("Id")]
        public int Id { get; set; }

        [CsvTo.CsvColumn("Name")]
        public string Name { get; set; } = string.Empty;

        [CsvTo.CsvColumn("Age")]
        public int Age { get; set; }

        [CsvTo.CsvColumn("Department")]
        public string Department { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"[{Id}] {Name} - {Age}岁, {Department}";
        }
    }
}
