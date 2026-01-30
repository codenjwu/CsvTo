using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    /// <summary>
    /// Complex scenario tests for real-world use cases
    /// </summary>
    [TestClass]
    public class ComplexScenarioTests
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

        #region Mixed Data Type Tests

        [TestMethod]
        public void TestMixedNumericFormats()
        {
            var csvContent = @"id,integer,float,scientific,percentage
1,1000,3.14159,1.23e-4,85%
2,-500,2.71828,6.02e23,120.5%
3,0,0.0,0.0e0,0%";

            var file = Path.Combine(_tempDir, "mixed_numeric.csv");
            File.WriteAllText(file, csvContent, Encoding.UTF8);

            var converter = new CsvTo.CsvConverter(file, hasHeader: true);
            var dt = converter.ToDataTable();

            Assert.AreEqual(3, dt.Rows.Count);
            Assert.AreEqual("1.23e-4", dt.Rows[0]["scientific"].ToString());
            Assert.AreEqual("85%", dt.Rows[0]["percentage"].ToString());
        }

        [TestMethod]
        public void TestMixedDateFormats()
        {
            var csvContent = @"id,date_iso,date_us,date_eu,timestamp
1,2024-01-15,01/15/2024,15/01/2024,2024-01-15 14:30:00
2,2024-12-31,12/31/2024,31/12/2024,2024-12-31 23:59:59";

            var file = Path.Combine(_tempDir, "mixed_dates.csv");
            File.WriteAllText(file, csvContent, Encoding.UTF8);

            var converter = new CsvTo.CsvConverter(file, hasHeader: true);
            var collection = converter.ToCollection().ToList();

            Assert.AreEqual(2, collection.Count);
            Assert.AreEqual("2024-01-15", collection[0][1]);
            Assert.AreEqual("2024-01-15 14:30:00", collection[0][4]);
        }

        [TestMethod]
        public void TestBooleanVariations()
        {
            var csvContent = @"id,bool1,bool2,bool3,bool4,bool5
1,true,True,TRUE,yes,1
2,false,False,FALSE,no,0
3,true,TRUE,true,Yes,1";

            var file = Path.Combine(_tempDir, "boolean_vars.csv");
            File.WriteAllText(file, csvContent, Encoding.UTF8);

            var converter = new CsvTo.CsvConverter(file, hasHeader: true);
            var dt = converter.ToDataTable();

            Assert.AreEqual(3, dt.Rows.Count);
            Assert.AreEqual("true", dt.Rows[0]["bool1"].ToString());
            Assert.AreEqual("yes", dt.Rows[0]["bool4"].ToString());
        }

        #endregion

        #region Special Characters and Encoding Tests

        [TestMethod]
        public void TestQuotedFieldsWithCommas()
        {
            var csvContent = @"id,name,address,description
1,""Smith, John"",""123 Main St, Apt 4"",""Works at ABC, Inc.""
2,""Doe, Jane"",""456 Oak Ave, Suite 200"",""CEO of XYZ, Corp.""";

            var file = Path.Combine(_tempDir, "quoted_commas.csv");
            File.WriteAllText(file, csvContent, Encoding.UTF8);

            var converter = new CsvTo.CsvConverter(file, hasHeader: true);
            var collection = converter.ToCollection().ToList();

            Assert.AreEqual(2, collection.Count);
            Assert.AreEqual("Smith, John", collection[0][1]);
            Assert.AreEqual("123 Main St, Apt 4", collection[0][2]);
            Assert.AreEqual("Works at ABC, Inc.", collection[0][3]);
        }

        [TestMethod]
        public void TestQuotedFieldsWithNewlines()
        {
            var csvContent = "id,comment\n1,\"Line 1\nLine 2\nLine 3\"\n2,\"Single line\"";

            var file = Path.Combine(_tempDir, "quoted_newlines.csv");
            File.WriteAllText(file, csvContent, Encoding.UTF8);

            var converter = new CsvTo.CsvConverter(file, hasHeader: true);
            var collection = converter.ToCollection().ToList();

            Assert.AreEqual(2, collection.Count);
            Assert.IsTrue(collection[0][1].Contains("Line 1"));
        }

        [TestMethod]
        public void TestEscapedQuotes()
        {
            var csvContent = "id,text\n1,\"He said \"\"Hello\"\"\"\n2,\"She replied \"\"Hi\"\"\"";

            var file = Path.Combine(_tempDir, "escaped_quotes.csv");
            File.WriteAllText(file, csvContent, Encoding.UTF8);

            var converter = new CsvTo.CsvConverter(file, hasHeader: true);
            var collection = converter.ToCollection().ToList();

            Assert.AreEqual(2, collection.Count);
            Assert.IsTrue(collection[0][1].Contains("Hello"));
        }

        [TestMethod]
        public void TestMultipleEncodingsInSequence()
        {
            // Test UTF-8
            var utf8File = Path.Combine(_tempDir, "utf8.csv");
            File.WriteAllText(utf8File, "名前,年齢\n太郎,25", Encoding.UTF8);

            // Test UTF-16
            var utf16File = Path.Combine(_tempDir, "utf16.csv");
            File.WriteAllText(utf16File, "名前,年齢\n太郎,25", Encoding.Unicode);

            var converter1 = new CsvTo.CsvConverter(utf8File, hasHeader: true, encoding: Encoding.UTF8);
            var dt1 = converter1.ToDataTable();

            var converter2 = new CsvTo.CsvConverter(utf16File, hasHeader: true, encoding: Encoding.Unicode);
            var dt2 = converter2.ToDataTable();

            Assert.IsNotNull(dt1);
            Assert.IsNotNull(dt2);
            Assert.AreEqual("名前", dt1.Columns[0].ColumnName);
            Assert.AreEqual("名前", dt2.Columns[0].ColumnName);
        }

        [TestMethod]
        public void TestEmojiAndSpecialUnicodeCharacters()
        {
            var csvContent = @"id,emoji,symbol,unicode
1,😀,★,♠
2,🎉,♥,♣
3,🚀,♦,♪";

            var file = Path.Combine(_tempDir, "unicode.csv");
            File.WriteAllText(file, csvContent, Encoding.UTF8);

            var converter = new CsvTo.CsvConverter(file, hasHeader: true, encoding: Encoding.UTF8);
            var collection = converter.ToCollection().ToList();

            Assert.AreEqual(3, collection.Count);
            Assert.AreEqual("😀", collection[0][1]);
            Assert.AreEqual("★", collection[0][2]);
        }

        #endregion

        #region Data Validation and Error Recovery Tests

        [TestMethod]
        public void TestInconsistentColumnCounts()
        {
            var csvContent = @"a,b,c
1,2,3
4,5
6,7,8,9";

            var file = Path.Combine(_tempDir, "inconsistent_columns.csv");
            File.WriteAllText(file, csvContent, Encoding.UTF8);

            var converter = new CsvTo.CsvConverter(file, hasHeader: true);
            var collection = converter.ToCollection().ToList();

            Assert.IsNotNull(collection);
            Assert.AreEqual(3, collection.Count);
            // Should handle rows with different column counts
        }

        [TestMethod]
        public void TestEmptyFieldsAndNulls()
        {
            var csvContent = @"id,name,value
1,,100
2,"""",200
3,name,";

            var file = Path.Combine(_tempDir, "empty_fields.csv");
            File.WriteAllText(file, csvContent, Encoding.UTF8);

            var converter = new CsvTo.CsvConverter(file, hasHeader: true);
            var dt = converter.ToDataTable();

            Assert.AreEqual(3, dt.Rows.Count);
            Assert.AreEqual("", dt.Rows[0]["name"].ToString());
            Assert.AreEqual("", dt.Rows[1]["name"].ToString());
            Assert.AreEqual("", dt.Rows[2]["value"].ToString());
        }

        [TestMethod]
        public void TestWhitespacePreservation()
        {
            var csvContent = @"id,text
1,  leading spaces
2,trailing spaces  
3,  both sides  ";

            var file = Path.Combine(_tempDir, "whitespace.csv");
            File.WriteAllText(file, csvContent, Encoding.UTF8);

            var converter = new CsvTo.CsvConverter(file, hasHeader: true);
            var collection = converter.ToCollection().ToList();

            Assert.AreEqual(3, collection.Count);
            Assert.IsTrue(collection[0][1].StartsWith("  "));
            Assert.IsTrue(collection[1][1].EndsWith("  "));
        }

        #endregion

        #region Performance and Scalability Tests

        [TestMethod]
        public void TestVeryLargeFile_100kRows()
        {
            var file = Path.Combine(_tempDir, "large_100k.csv");
            using (var writer = new StreamWriter(file, false, Encoding.UTF8))
            {
                writer.WriteLine("id,name,value,timestamp");
                for (int i = 0; i < 100000; i++)
                {
                    writer.WriteLine($"{i},name{i},{i * 1.5},2024-01-{(i % 28) + 1:D2}");
                }
            }

            var converter = new CsvTo.CsvConverter(file, hasHeader: true);
            var collection = converter.ToCollection().ToList();

            Assert.AreEqual(100000, collection.Count);
            Assert.AreEqual("0", collection[0][0]);
            Assert.AreEqual("99999", collection[99999][0]);
        }

        [TestMethod]
        public void TestWideFile_ManyColumns()
        {
            var columns = string.Join(",", Enumerable.Range(1, 100).Select(i => $"col{i}"));
            var data = string.Join(",", Enumerable.Range(1, 100).Select(i => i.ToString()));

            var csvContent = $"{columns}\n{data}\n{data}";
            var file = Path.Combine(_tempDir, "wide_file.csv");
            File.WriteAllText(file, csvContent, Encoding.UTF8);

            var converter = new CsvTo.CsvConverter(file, hasHeader: true);
            var dt = converter.ToDataTable();

            Assert.AreEqual(100, dt.Columns.Count);
            Assert.AreEqual(2, dt.Rows.Count);
            Assert.AreEqual("col1", dt.Columns[0].ColumnName);
            Assert.AreEqual("col100", dt.Columns[99].ColumnName);
        }

        [TestMethod]
        public void TestRepeatedConversionSameFile()
        {
            var file = Path.Combine(_tempDir, "repeated.csv");
            File.WriteAllText(file, "a,b,c\n1,2,3", Encoding.UTF8);

            var converter = new CsvTo.CsvConverter(file, hasHeader: true);

            for (int i = 0; i < 10; i++)
            {
                var collection = converter.ToCollection().ToList();
                Assert.AreEqual(1, collection.Count);
                Assert.AreEqual("1", collection[0][0]);
            }
        }

        #endregion

        #region Concurrent Access Tests

        [TestMethod]
        public void TestConcurrentReadsFromSameFile()
        {
            var file = Path.Combine(_tempDir, "concurrent.csv");
            using (var writer = new StreamWriter(file, false, Encoding.UTF8))
            {
                writer.WriteLine("id,value");
                for (int i = 0; i < 1000; i++)
                {
                    writer.WriteLine($"{i},{i * 100}");
                }
            }

            var tasks = new List<Task<int>>();
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    var converter = new CsvTo.CsvConverter(file, hasHeader: true);
                    var collection = converter.ToCollection().ToList();
                    return collection.Count;
                }));
            }

            Task.WaitAll(tasks.ToArray());
            Assert.IsTrue(tasks.All(t => t.Result == 1000));
        }

        [TestMethod]
        public void TestConcurrentDifferentFiles()
        {
            var files = new List<string>();
            for (int i = 0; i < 5; i++)
            {
                var file = Path.Combine(_tempDir, $"file{i}.csv");
                File.WriteAllText(file, $"id,value\n{i},{i * 100}", Encoding.UTF8);
                files.Add(file);
            }

            var tasks = files.Select((file, index) => Task.Run(() =>
            {
                var converter = new CsvTo.CsvConverter(file, hasHeader: true);
                var collection = converter.ToCollection().ToList();
                return (index, collection[0][0]);
            })).ToList();

            Task.WaitAll(tasks.ToArray());
            
            foreach (var task in tasks)
            {
                var (index, value) = task.Result;
                Assert.AreEqual(index.ToString(), value);
            }
        }

        #endregion

        #region Generic Type Advanced Tests

        [TestMethod]
        public void TestComplexNestedProperties()
        {
            var csvContent = @"id,name,salary,hireDate,isActive
1,Alice,75000.50,2020-01-15,true
2,Bob,85000.75,2019-06-20,false";

            var file = Path.Combine(_tempDir, "employees.csv");
            File.WriteAllText(file, csvContent, Encoding.UTF8);

            var converter = new CsvTo.CsvConverter<ComplexEmployee>(file);
            var employees = converter.ToCollection().ToList();

            Assert.AreEqual(2, employees.Count);
            Assert.AreEqual(1, employees[0].id);
            Assert.AreEqual("Alice", employees[0].name);
        }

        [TestMethod]
        public void TestNullableTypesConversion()
        {
            var csvContent = @"id,value1,value2,value3
1,100,,
2,,200,
3,,,300";

            var file = Path.Combine(_tempDir, "nullables.csv");
            File.WriteAllText(file, csvContent, Encoding.UTF8);

            var converter = new CsvTo.CsvConverter<NullableModel>(file);
            var items = converter.ToCollection().ToList();

            Assert.AreEqual(3, items.Count);
            Assert.AreEqual(100, items[0].value1);
            Assert.IsNull(items[0].value2);
            Assert.IsNull(items[1].value1);
            Assert.AreEqual(200, items[1].value2);
        }

        [TestMethod]
        public void TestInheritedProperties()
        {
            var csvContent = @"id,name,baseProperty,derivedProperty
1,Test,BaseValue,DerivedValue";

            var file = Path.Combine(_tempDir, "inherited.csv");
            File.WriteAllText(file, csvContent, Encoding.UTF8);

            var converter = new CsvTo.CsvConverter<DerivedModel>(file);
            var items = converter.ToCollection().ToList();

            Assert.AreEqual(1, items.Count);
            Assert.AreEqual(1, items[0].id);
            Assert.AreEqual("Test", items[0].name);
        }

        #endregion

        #region Multi-delimiter and Format Tests

        [TestMethod]
        public void TestPipeDelimiterWithComplexData()
        {
            var csvContent = "id|name|description|tags\n1|ProductA|HighQuality|tag1\n2|ProductB|Lightweight|tag4";

            var file = Path.Combine(_tempDir, "pipe_delimited.csv");
            File.WriteAllText(file, csvContent, Encoding.UTF8);

            var converter = new CsvTo.CsvConverter(file, hasHeader: true, delimiter: "|");
            var collection = converter.ToCollection().ToList();

            Assert.AreEqual(2, collection.Count);
            Assert.AreEqual("ProductA", collection[0][1]);
            Assert.AreEqual("HighQuality", collection[0][2]);
        }

        [TestMethod]
        public void TestCustomDelimiterColon()
        {
            // Note: Library only supports single-character delimiters
            var csvContent = "id:name:value\n1:Alice:100\n2:Bob:200";

            var file = Path.Combine(_tempDir, "colon_delimiter.csv");
            File.WriteAllText(file, csvContent, Encoding.UTF8);

            var converter = new CsvTo.CsvConverter(file, hasHeader: true, delimiter: ":");
            var collection = converter.ToCollection().ToList();

            Assert.AreEqual(2, collection.Count);
            Assert.AreEqual("1", collection[0][0]);
            Assert.AreEqual("Alice", collection[0][1]);
        }

        #endregion

        #region Stream and Memory Tests

        [TestMethod]
        public void TestLargeMemoryStream()
        {
            var sb = new StringBuilder();
            sb.AppendLine("id,value");
            for (int i = 0; i < 10000; i++)
            {
                sb.AppendLine($"{i},{i * 100}");
            }

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString())))
            {
                var converter = new CsvTo.CsvConverter(stream, hasHeader: true);
                var collection = converter.ToCollection().ToList();

                Assert.AreEqual(10000, collection.Count);
            }
        }

        [TestMethod]
        public void TestStreamWithBOM()
        {
            var csvContent = "id,name\n1,Test";
            var bytes = Encoding.UTF8.GetPreamble()
                .Concat(Encoding.UTF8.GetBytes(csvContent))
                .ToArray();

            using (var stream = new MemoryStream(bytes))
            {
                var converter = new CsvTo.CsvConverter(stream, hasHeader: true, encoding: Encoding.UTF8);
                var collection = converter.ToCollection().ToList();

                Assert.AreEqual(1, collection.Count);
                Assert.AreEqual("1", collection[0][0]);
            }
        }

        #endregion

        #region Real-World Scenario Tests

        [TestMethod]
        public void TestLogFileProcessing()
        {
            var csvContent = @"timestamp,level,message,source
2024-01-15 10:30:15,INFO,Application started,Main
2024-01-15 10:30:16,DEBUG,Loading configuration,ConfigLoader
2024-01-15 10:30:17,WARN,Connection timeout,Database
2024-01-15 10:30:18,ERROR,Failed to connect,Database";

            var file = Path.Combine(_tempDir, "log.csv");
            File.WriteAllText(file, csvContent, Encoding.UTF8);

            // Read log in reverse order (most recent first)
            var converter = new CsvTo.CsvReverseConverter(file, hasHeader: true);
            var logs = converter.ToCollection().ToList();

            Assert.AreEqual(4, logs.Count);
            Assert.AreEqual("ERROR", logs[0][1]); // Most recent log level
            Assert.AreEqual("INFO", logs[3][1]);  // Oldest log level
        }

        [TestMethod]
        public void TestFinancialDataProcessing()
        {
            var csvContent = @"date,symbol,open,high,low,close,volume
2024-01-15,AAPL,150.25,152.50,149.80,151.75,1250000
2024-01-15,MSFT,380.00,385.25,378.50,382.90,980000
2024-01-15,GOOGL,140.50,142.00,139.75,141.25,750000";

            var file = Path.Combine(_tempDir, "stocks.csv");
            File.WriteAllText(file, csvContent, Encoding.UTF8);

            var converter = new CsvTo.CsvConverter(file, hasHeader: true);
            var stocks = converter.ToCollection().ToList();

            Assert.AreEqual(3, stocks.Count);
            
            // Verify financial data precision
            Assert.AreEqual("150.25", stocks[0][2]);
            Assert.AreEqual("1250000", stocks[0][6]);
        }

        [TestMethod]
        public void TestMultiLanguageContactList()
        {
            var csvContent = @"id,姓名,Email,电话,城市
1,张三,zhang@test.com,13800138000,北京
2,李四,li@test.com,13900139000,上海
3,王五,wang@test.com,13700137000,深圳";

            var file = Path.Combine(_tempDir, "contacts.csv");
            File.WriteAllText(file, csvContent, Encoding.UTF8);

            var converter = new CsvTo.CsvConverter(file, hasHeader: true, encoding: Encoding.UTF8);
            var dt = converter.ToDataTable();

            Assert.AreEqual(3, dt.Rows.Count);
            Assert.AreEqual("姓名", dt.Columns[1].ColumnName);
            Assert.AreEqual("张三", dt.Rows[0][1].ToString());
        }

        #endregion
    }

    #region Test Models

    public class ComplexEmployee
    {
        public int id { get; set; }
        public string name { get; set; }
        public decimal salary { get; set; }
        public DateTime hireDate { get; set; }
        public bool isActive { get; set; }
    }

    public class NullableModel
    {
        public int id { get; set; }
        public int? value1 { get; set; }
        public int? value2 { get; set; }
        public int? value3 { get; set; }
    }

    public class BaseModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string baseProperty { get; set; }
    }

    public class DerivedModel : BaseModel
    {
        public string derivedProperty { get; set; }
    }

    #endregion
}
