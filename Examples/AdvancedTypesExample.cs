using System;
using System.Collections.Generic;
using System.Linq;

namespace CsvToExamples
{
    public static class AdvancedTypesExample
    {
        public static void Run()
        {
            Console.WriteLine("=== 示例 8: 高级用法 - 复杂类型转换 ===\n");

            string csvFile = "employees.csv";
            CreateEmployeeCsv(csvFile);

            try
            {
                // 泛型转换到复杂类型
                var converter = new CsvTo.CsvConverter<Employee>(csvFile);
                var employees = converter.ToCollection().ToList();

                Console.WriteLine("员工信息:\n");
                foreach (var emp in employees)
                {
                    Console.WriteLine(emp);
                    Console.WriteLine($"  入职天数: {emp.DaysEmployed}");
                    Console.WriteLine($"  月薪: ${emp.MonthlySalary:N2}");
                    Console.WriteLine();
                }

                // 数据分析
                Console.WriteLine("=== 数据分析 ===");
                Console.WriteLine($"总员工数: {employees.Count}");
                Console.WriteLine($"平均薪资: ${employees.Average(e => e.Salary):N2}");
                Console.WriteLine($"最高薪资: ${employees.Max(e => e.Salary):N2}");
                Console.WriteLine($"最低薪资: ${employees.Min(e => e.Salary):N2}");

                var activeEmployees = employees.Count(e => e.IsActive);
                Console.WriteLine($"\n在职员工: {activeEmployees}");
                Console.WriteLine($"离职员工: {employees.Count - activeEmployees}");

                // 按部门分组
                Console.WriteLine("\n按部门统计:");
                var byDepartment = employees.GroupBy(e => e.Department);
                foreach (var group in byDepartment)
                {
                    Console.WriteLine($"  {group.Key}: {group.Count()} 人, 平均薪资: ${group.Average(e => e.Salary):N2}");
                }

                // 可空类型处理
                Console.WriteLine("\n可空类型处理:");
                var withBonus = employees.Count(e => e.Bonus.HasValue);
                Console.WriteLine($"有奖金的员工: {withBonus}");
                if (withBonus > 0)
                {
                    var avgBonus = employees.Where(e => e.Bonus.HasValue).Average(e => e.Bonus!.Value);
                    Console.WriteLine($"平均奖金: ${avgBonus:N2}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"错误: {ex.Message}");
            }
        }

        private static void CreateEmployeeCsv(string filename)
        {
            var lines = new[]
            {
                "Id,Name,Department,Salary,HireDate,IsActive,Bonus",
                "1,张三,Engineering,75000.50,2020-01-15,true,5000",
                "2,李四,Marketing,65000.00,2019-03-20,true,",
                "3,王五,Engineering,80000.75,2021-06-10,true,7500",
                "4,赵六,Sales,70000.00,2018-11-05,false,",
                "5,钱七,HR,60000.25,2022-02-28,true,3000"
            };
            System.IO.File.WriteAllLines(filename, lines);
        }
    }

    public class Employee
    {
        [CsvTo.CsvColumn("Id")]
        public int Id { get; set; }

        [CsvTo.CsvColumn("Name")]
        public string Name { get; set; } = string.Empty;

        [CsvTo.CsvColumn("Department")]
        public string Department { get; set; } = string.Empty;

        [CsvTo.CsvColumn("Salary")]
        public decimal Salary { get; set; }

        [CsvTo.CsvColumn("HireDate")]
        public DateTime HireDate { get; set; }

        [CsvTo.CsvColumn("IsActive")]
        public bool IsActive { get; set; }

        [CsvTo.CsvColumn("Bonus")]
        public decimal? Bonus { get; set; }  // 可空类型

        // 计算属性
        public int DaysEmployed => (DateTime.Now - HireDate).Days;
        public decimal MonthlySalary => Salary / 12;

        public override string ToString()
        {
            var status = IsActive ? "在职" : "离职";
            var bonusInfo = Bonus.HasValue ? $", 奖金: ${Bonus:N2}" : "";
            return $"[{Id}] {Name} - {Department}, 薪资: ${Salary:N2}, 入职日期: {HireDate:yyyy-MM-dd}, 状态: {status}{bonusInfo}";
        }
    }
}
