using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace UnitTest
{
    /// <summary>
    /// Advanced performance and memory tests
    /// </summary>
    [TestClass]
    public class AdvancedPerformanceTests
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
        public void TestStreamingLargeFile_MemoryEfficiency()
        {
            var file = Path.Combine(_tempDir, "large_stream.csv");
            using (var writer = new StreamWriter(file, false, Encoding.UTF8))
            {
                writer.WriteLine("id,data");
                for (int i = 0; i < 50000; i++)
                {
                    writer.WriteLine($"{i},{new string('x', 100)}");
                }
            }

            var initialMemory = GC.GetTotalMemory(true);
            
            var converter = new CsvTo.CsvConverter(file, hasHeader: true);
            
            // Use enumeration instead of ToList to test streaming
            int count = 0;
            foreach (var row in converter.ToCollection())
            {
                count++;
                if (count % 10000 == 0)
                {
                    // Force garbage collection periodically
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }

            var finalMemory = GC.GetTotalMemory(true);
            var memoryUsed = (finalMemory - initialMemory) / 1024 / 1024; // MB

            Assert.AreEqual(50000, count);
            // Memory usage should be reasonable (less than 100MB for this test)
            Assert.IsTrue(memoryUsed < 100, $"Memory used: {memoryUsed}MB");
        }

        [TestMethod]
        public void TestConversionSpeed_SmallFiles()
        {
            var file = Path.Combine(_tempDir, "speed_small.csv");
            using (var writer = new StreamWriter(file, false, Encoding.UTF8))
            {
                writer.WriteLine("id,name,value");
                for (int i = 0; i < 100; i++)
                {
                    writer.WriteLine($"{i},name{i},{i * 100}");
                }
            }

            var sw = Stopwatch.StartNew();
            var converter = new CsvTo.CsvConverter(file, hasHeader: true);
            var collection = converter.ToCollection().ToList();
            sw.Stop();

            Assert.AreEqual(100, collection.Count);
            // Should complete in less than 100ms for small files
            Assert.IsTrue(sw.ElapsedMilliseconds < 100, $"Took {sw.ElapsedMilliseconds}ms");
        }

        [TestMethod]
        public void TestConversionSpeed_MediumFiles()
        {
            var file = Path.Combine(_tempDir, "speed_medium.csv");
            using (var writer = new StreamWriter(file, false, Encoding.UTF8))
            {
                writer.WriteLine("id,name,value,timestamp");
                for (int i = 0; i < 10000; i++)
                {
                    writer.WriteLine($"{i},name{i},{i * 100},2024-01-15");
                }
            }

            var sw = Stopwatch.StartNew();
            var converter = new CsvTo.CsvConverter(file, hasHeader: true);
            var collection = converter.ToCollection().ToList();
            sw.Stop();

            Assert.AreEqual(10000, collection.Count);
            // Should complete in reasonable time (less than 2 seconds)
            Assert.IsTrue(sw.ElapsedMilliseconds < 2000, $"Took {sw.ElapsedMilliseconds}ms");
        }

        [TestMethod]
        public void TestDataTableConversionSpeed()
        {
            var file = Path.Combine(_tempDir, "dt_speed.csv");
            using (var writer = new StreamWriter(file, false, Encoding.UTF8))
            {
                writer.WriteLine("col1,col2,col3,col4,col5");
                for (int i = 0; i < 5000; i++)
                {
                    writer.WriteLine($"{i},{i * 2},{i * 3},{i * 4},{i * 5}");
                }
            }

            var sw = Stopwatch.StartNew();
            var converter = new CsvTo.CsvConverter(file, hasHeader: true);
            var dt = converter.ToDataTable();
            sw.Stop();

            Assert.AreEqual(5000, dt.Rows.Count);
            Assert.IsTrue(sw.ElapsedMilliseconds < 1000, $"Took {sw.ElapsedMilliseconds}ms");
        }

        [TestMethod]
        public void TestGenericConversionSpeed()
        {
            var csvContent = @"id,name,value
1,Alice,100
2,Bob,200
3,Charlie,300";

            var file = Path.Combine(_tempDir, "generic_speed.csv");
            File.WriteAllText(file, csvContent, Encoding.UTF8);

            var sw = Stopwatch.StartNew();
            var converter = new CsvTo.CsvConverter<SimpleModel>(file);
            var items = converter.ToCollection().ToList();
            sw.Stop();

            Assert.AreEqual(3, items.Count);
            Assert.IsTrue(sw.ElapsedMilliseconds < 50, $"Took {sw.ElapsedMilliseconds}ms");
        }

        [TestMethod]
        public void TestReverseConversionSpeed()
        {
            var file = Path.Combine(_tempDir, "reverse_speed.csv");
            using (var writer = new StreamWriter(file, false, Encoding.UTF8))
            {
                writer.WriteLine("id,value");
                for (int i = 0; i < 10000; i++)
                {
                    writer.WriteLine($"{i},{i * 100}");
                }
            }

            var sw = Stopwatch.StartNew();
            var converter = new CsvTo.CsvReverseConverter(file, hasHeader: true);
            var collection = converter.ToCollection().ToList();
            sw.Stop();

            Assert.AreEqual(10000, collection.Count);
            Assert.AreEqual("9999", collection[0][0]); // Last row first
            Assert.IsTrue(sw.ElapsedMilliseconds < 2000, $"Took {sw.ElapsedMilliseconds}ms");
        }

        [TestMethod]
        public void TestMultipleFileConversionsSequential()
        {
            var files = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                var file = Path.Combine(_tempDir, $"multi_{i}.csv");
                using (var writer = new StreamWriter(file, false, Encoding.UTF8))
                {
                    writer.WriteLine("id,value");
                    for (int j = 0; j < 1000; j++)
                    {
                        writer.WriteLine($"{j},{j * 100}");
                    }
                }
                files.Add(file);
            }

            var sw = Stopwatch.StartNew();
            var totalRows = 0;
            foreach (var file in files)
            {
                var converter = new CsvTo.CsvConverter(file, hasHeader: true);
                totalRows += converter.ToCollection().Count();
            }
            sw.Stop();

            Assert.AreEqual(10000, totalRows);
            Assert.IsTrue(sw.ElapsedMilliseconds < 3000, $"Took {sw.ElapsedMilliseconds}ms");
        }

        [TestMethod]
        public void TestWideFilePerformance()
        {
            var file = Path.Combine(_tempDir, "wide_perf.csv");
            using (var writer = new StreamWriter(file, false, Encoding.UTF8))
            {
                // Create header with 200 columns
                var headers = string.Join(",", Enumerable.Range(1, 200).Select(i => $"col{i}"));
                writer.WriteLine(headers);

                // Write 1000 rows
                for (int i = 0; i < 1000; i++)
                {
                    var data = string.Join(",", Enumerable.Range(1, 200).Select(j => $"val{j}"));
                    writer.WriteLine(data);
                }
            }

            var sw = Stopwatch.StartNew();
            var converter = new CsvTo.CsvConverter(file, hasHeader: true);
            var dt = converter.ToDataTable();
            sw.Stop();

            Assert.AreEqual(200, dt.Columns.Count);
            Assert.AreEqual(1000, dt.Rows.Count);
            Assert.IsTrue(sw.ElapsedMilliseconds < 2000, $"Took {sw.ElapsedMilliseconds}ms");
        }

        [TestMethod]
        public void TestEncodingConversionOverhead()
        {
            var csvContent = "id,text\n1,Hello\n2,World";

            // Test UTF-8
            var utf8File = Path.Combine(_tempDir, "utf8_perf.csv");
            File.WriteAllText(utf8File, csvContent, Encoding.UTF8);

            var sw1 = Stopwatch.StartNew();
            var converter1 = new CsvTo.CsvConverter(utf8File, hasHeader: true, encoding: Encoding.UTF8);
            var result1 = converter1.ToCollection().ToList();
            sw1.Stop();

            // Test UTF-16
            var utf16File = Path.Combine(_tempDir, "utf16_perf.csv");
            File.WriteAllText(utf16File, csvContent, Encoding.Unicode);

            var sw2 = Stopwatch.StartNew();
            var converter2 = new CsvTo.CsvConverter(utf16File, hasHeader: true, encoding: Encoding.Unicode);
            var result2 = converter2.ToCollection().ToList();
            sw2.Stop();

            Assert.AreEqual(result1.Count, result2.Count);
            // Both should complete quickly
            Assert.IsTrue(sw1.ElapsedMilliseconds < 100);
            Assert.IsTrue(sw2.ElapsedMilliseconds < 100);
        }

        [TestMethod]
        public void TestRepeatedAccessPerformance()
        {
            var file = Path.Combine(_tempDir, "repeated_access.csv");
            using (var writer = new StreamWriter(file, false, Encoding.UTF8))
            {
                writer.WriteLine("id,value");
                for (int i = 0; i < 5000; i++)
                {
                    writer.WriteLine($"{i},{i * 100}");
                }
            }

            var converter = new CsvTo.CsvConverter(file, hasHeader: true);
            var times = new List<long>();

            // Access same converter multiple times
            for (int i = 0; i < 5; i++)
            {
                var sw = Stopwatch.StartNew();
                var collection = converter.ToCollection().ToList();
                sw.Stop();
                times.Add(sw.ElapsedMilliseconds);

                Assert.AreEqual(5000, collection.Count);
            }

            // All accesses should be consistent
            var avgTime = times.Average();
            Assert.IsTrue(avgTime < 1000, $"Average time: {avgTime}ms");
        }
    }

    public class SimpleModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public int value { get; set; }
    }
}
