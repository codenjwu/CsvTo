using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace UnitTest
{
    /// <summary>
    /// Integration tests for end-to-end scenarios
    /// </summary>
    [TestClass]
    public class IntegrationTests
    {
        private string _tempDir;

        [TestInitialize]
        public void Setup()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDir);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(_tempDir))
            {
                Directory.Delete(_tempDir, true);
            }
        }

        [TestMethod]
        public void TestDataPipeline_CsvToDataTableToAnalysis()
        {
            // Step 1: Create CSV with sales data
            var csvContent = @"date,product,quantity,price,region
2024-01-01,ProductA,10,99.99,North
2024-01-02,ProductB,5,149.99,South
2024-01-03,ProductA,8,99.99,East
2024-01-04,ProductC,12,79.99,North
2024-01-05,ProductB,15,149.99,West";

            var file = Path.Combine(_tempDir, "sales.csv");
            File.WriteAllText(file, csvContent, Encoding.UTF8);

            // Step 2: Convert to DataTable
            var converter = new CsvTo.CsvConverter(file, hasHeader: true);
            var dt = converter.ToDataTable();

            // Step 3: Perform analysis
            Assert.AreEqual(5, dt.Rows.Count);
            
            // Calculate total revenue
            decimal totalRevenue = 0;
            foreach (DataRow row in dt.Rows)
            {
                var quantity = int.Parse(row["quantity"].ToString());
                var price = decimal.Parse(row["price"].ToString());
                totalRevenue += quantity * price;
            }

            // Verify calculations
            Assert.IsTrue(totalRevenue > 0);
            decimal expected = (10 * 99.99m) + (5 * 149.99m) + (8 * 99.99m) + (12 * 79.99m) + (15 * 149.99m);
            Assert.AreEqual(expected, totalRevenue, 0.01m);
        }

        [TestMethod]
        public void TestDataTransformation_GenericTypesWithValidation()
        {
            var csvContent = @"id,email,age,salary,joinDate
1,alice@test.com,25,50000,2020-01-15
2,bob@test.com,30,60000,2019-05-20
3,charlie@test.com,28,55000,2021-03-10";

            var file = Path.Combine(_tempDir, "employees.csv");
            File.WriteAllText(file, csvContent, Encoding.UTF8);

            var converter = new CsvTo.CsvConverter<EmployeeModel>(file);
            var employees = converter.ToCollection().ToList();

            // Validation
            Assert.AreEqual(3, employees.Count);
            Assert.IsTrue(employees.All(e => e.email.Contains("@")));
            Assert.IsTrue(employees.All(e => e.age > 18));
            Assert.IsTrue(employees.All(e => e.salary > 0));
            Assert.IsTrue(employees.All(e => e.joinDate < DateTime.Now));

            // Business logic
            var avgSalary = employees.Average(e => e.salary);
            Assert.IsTrue(avgSalary > 50000);
        }

        [TestMethod]
        public void TestMultiFileAggregation()
        {
            // Create multiple CSV files representing different months
            var months = new[] { "Jan", "Feb", "Mar" };
            var allData = new List<string[]>();

            foreach (var month in months)
            {
                var csvContent = $@"month,sales
{month},1000
{month},1500
{month},2000";

                var file = Path.Combine(_tempDir, $"sales_{month}.csv");
                File.WriteAllText(file, csvContent, Encoding.UTF8);

                var converter = new CsvTo.CsvConverter(file, hasHeader: true);
                allData.AddRange(converter.ToCollection().ToList());
            }

            // Aggregate data
            Assert.AreEqual(9, allData.Count); // 3 rows × 3 files
            
            var totalSales = allData.Sum(row => int.Parse(row[1]));
            Assert.AreEqual(13500, totalSales); // (1000+1500+2000) × 3
        }

        [TestMethod]
        public void TestDataMerging_MultipleSourcesWithJoin()
        {
            // Customer file
            var customersContent = @"id,name,city
1,Alice,New York
2,Bob,Los Angeles
3,Charlie,Chicago";

            var customersFile = Path.Combine(_tempDir, "customers.csv");
            File.WriteAllText(customersFile, customersContent, Encoding.UTF8);

            // Orders file
            var ordersContent = @"orderId,customerId,amount
101,1,500
102,2,750
103,1,300
104,3,1000";

            var ordersFile = Path.Combine(_tempDir, "orders.csv");
            File.WriteAllText(ordersFile, ordersContent, Encoding.UTF8);

            // Load both files
            var customerConverter = new CsvTo.CsvConverter(customersFile, hasHeader: true);
            var ordersConverter = new CsvTo.CsvConverter(ordersFile, hasHeader: true);

            var customers = customerConverter.ToCollection().ToList();
            var orders = ordersConverter.ToCollection().ToList();

            // Perform join-like operation
            var customerOrders = from order in orders
                                 join customer in customers
                                 on order[1] equals customer[0]
                                 select new
                                 {
                                     OrderId = order[0],
                                     CustomerName = customer[1],
                                     City = customer[2],
                                     Amount = decimal.Parse(order[2])
                                 };

            var result = customerOrders.ToList();
            Assert.AreEqual(4, result.Count);
            Assert.AreEqual(2, result.Count(r => r.CustomerName == "Alice"));
            Assert.AreEqual(800, result.Where(r => r.CustomerName == "Alice").Sum(r => r.Amount));
        }

        [TestMethod]
        public void TestFilteringAndTransformation()
        {
            var csvContent = @"id,name,score,status
1,Alice,85,pass
2,Bob,45,fail
3,Charlie,92,pass
4,Diana,58,fail
5,Eve,78,pass";

            var file = Path.Combine(_tempDir, "students.csv");
            File.WriteAllText(file, csvContent, Encoding.UTF8);

            var converter = new CsvTo.CsvConverter(file, hasHeader: true);
            var students = converter.ToCollection().ToList();

            // Filter passing students
            var passedStudents = students.Where(s => s[3] == "pass").ToList();
            Assert.AreEqual(3, passedStudents.Count);

            // Filter high scorers (>80)
            var highScorers = students.Where(s => int.Parse(s[2]) > 80).ToList();
            Assert.AreEqual(2, highScorers.Count);

            // Calculate average score
            var avgScore = students.Average(s => int.Parse(s[2]));
            Assert.AreEqual(71.6, avgScore, 0.1);
        }

        [TestMethod]
        public void TestExportAndReimport()
        {
            // Original CSV
            var originalContent = @"id,name,value
1,Test1,100
2,Test2,200
3,Test3,300";

            var originalFile = Path.Combine(_tempDir, "original.csv");
            File.WriteAllText(originalFile, originalContent, Encoding.UTF8);

            // Read original
            var converter1 = new CsvTo.CsvConverter(originalFile, hasHeader: true);
            var dt1 = converter1.ToDataTable();

            // Export to new CSV (simulated)
            var exportFile = Path.Combine(_tempDir, "export.csv");
            using (var writer = new StreamWriter(exportFile, false, Encoding.UTF8))
            {
                // Write header
                var headers = new List<string>();
                foreach (DataColumn col in dt1.Columns)
                {
                    headers.Add(col.ColumnName);
                }
                writer.WriteLine(string.Join(",", headers));

                // Write rows
                foreach (DataRow row in dt1.Rows)
                {
                    writer.WriteLine(string.Join(",", row.ItemArray));
                }
            }

            // Re-import
            var converter2 = new CsvTo.CsvConverter(exportFile, hasHeader: true);
            var dt2 = converter2.ToDataTable();

            // Verify round-trip
            Assert.AreEqual(dt1.Rows.Count, dt2.Rows.Count);
            Assert.AreEqual(dt1.Columns.Count, dt2.Columns.Count);
        }

        [TestMethod]
        public void TestComplexBusinessRules()
        {
            var csvContent = @"transactionId,date,amount,type,category
1,2024-01-15,100.50,debit,groceries
2,2024-01-16,2500.00,credit,salary
3,2024-01-17,50.25,debit,transportation
4,2024-01-18,1200.00,debit,rent
5,2024-01-19,75.00,debit,utilities";

            var file = Path.Combine(_tempDir, "transactions.csv");
            File.WriteAllText(file, csvContent, Encoding.UTF8);

            var converter = new CsvTo.CsvConverter(file, hasHeader: true);
            var transactions = converter.ToCollection().ToList();

            // Calculate balance
            decimal balance = 0;
            foreach (var tx in transactions)
            {
                var amount = decimal.Parse(tx[2]);
                if (tx[3] == "credit")
                    balance += amount;
                else if (tx[3] == "debit")
                    balance -= amount;
            }

            Assert.AreEqual(1074.25m, balance, 0.01m);

            // Find largest expense
            var expenses = transactions
                .Where(t => t[3] == "debit")
                .Select(t => new { Category = t[4], Amount = decimal.Parse(t[2]) })
                .OrderByDescending(e => e.Amount)
                .ToList();

            Assert.AreEqual("rent", expenses[0].Category);
            Assert.AreEqual(1200.00m, expenses[0].Amount);
        }

        [TestMethod]
        public void TestTimeSeriesAnalysis()
        {
            var csvContent = @"timestamp,sensor,temperature,humidity
2024-01-15 10:00:00,Sensor1,22.5,45
2024-01-15 10:05:00,Sensor1,23.0,46
2024-01-15 10:10:00,Sensor1,23.5,47
2024-01-15 10:15:00,Sensor1,24.0,48
2024-01-15 10:20:00,Sensor1,24.5,49";

            var file = Path.Combine(_tempDir, "sensor_data.csv");
            File.WriteAllText(file, csvContent, Encoding.UTF8);

            var converter = new CsvTo.CsvConverter(file, hasHeader: true);
            var readings = converter.ToCollection().ToList();

            // Calculate average temperature
            var avgTemp = readings.Average(r => double.Parse(r[2]));
            Assert.AreEqual(23.5, avgTemp, 0.01);

            // Calculate temperature trend
            var firstTemp = double.Parse(readings[0][2]);
            var lastTemp = double.Parse(readings[4][2]);
            var trend = lastTemp - firstTemp;
            Assert.AreEqual(2.0, trend, 0.01);
        }

        [TestMethod]
        public void TestDataQualityCheck()
        {
            var csvContent = @"id,email,phone,age
1,alice@test.com,1234567890,25
2,bob@invalid,9876543210,30
3,charlie@test.com,,28
4,diana@test.com,5555555555,-5
5,eve@test.com,1111111111,150";

            var file = Path.Combine(_tempDir, "contacts.csv");
            File.WriteAllText(file, csvContent, Encoding.UTF8);

            var converter = new CsvTo.CsvConverter(file, hasHeader: true);
            var contacts = converter.ToCollection().ToList();

            // Validate data quality
            var validEmails = contacts.Count(c => c[1].Contains("@") && c[1].Contains("."));
            Assert.AreEqual(4, validEmails);

            var withPhone = contacts.Count(c => !string.IsNullOrWhiteSpace(c[2]));
            Assert.AreEqual(4, withPhone);

            var validAges = contacts.Count(c =>
            {
                if (int.TryParse(c[3], out int age))
                    return age > 0 && age < 120;
                return false;
            });
            Assert.AreEqual(3, validAges);
        }

        [TestMethod]
        public void TestHierarchicalDataProcessing()
        {
            var csvContent = @"department,employee,salary,manager
Sales,Alice,50000,
Sales,Bob,45000,Alice
Sales,Charlie,42000,Alice
IT,Diana,70000,
IT,Eve,65000,Diana
IT,Frank,60000,Diana";

            var file = Path.Combine(_tempDir, "org.csv");
            File.WriteAllText(file, csvContent, Encoding.UTF8);

            var converter = new CsvTo.CsvConverter(file, hasHeader: true);
            var employees = converter.ToCollection().ToList();

            // Group by department
            var departments = employees.GroupBy(e => e[0]).ToList();
            Assert.AreEqual(2, departments.Count);

            // Calculate department totals
            var salesTotal = employees
                .Where(e => e[0] == "Sales")
                .Sum(e => decimal.Parse(e[2]));
            Assert.AreEqual(137000m, salesTotal);

            var itTotal = employees
                .Where(e => e[0] == "IT")
                .Sum(e => decimal.Parse(e[2]));
            Assert.AreEqual(195000m, itTotal);

            // Find managers
            var managers = employees.Where(e => string.IsNullOrWhiteSpace(e[3])).ToList();
            Assert.AreEqual(2, managers.Count);
        }
    }

    public class EmployeeModel
    {
        public int id { get; set; }
        public string email { get; set; }
        public int age { get; set; }
        public decimal salary { get; set; }
        public DateTime joinDate { get; set; }
    }
}
