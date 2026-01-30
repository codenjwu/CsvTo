using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace UnitTest
{
    [TestClass]
    public class EdgeCaseTests
    {
        [TestMethod]
        public void TestEmptyFile()
        {
            var file = @"test_empty.csv";
            var converter = new CsvTo.CsvConverter(file);
            
            var dt = converter.ToDataTable();
            
            Assert.IsNotNull(dt);
            Assert.AreEqual(0, dt.Rows.Count);
        }

        [TestMethod]
        public void TestSingleLineHeaderOnly()
        {
            var file = @"test_single_line.csv";
            var converter = new CsvTo.CsvConverter(file, hasHeader: true);
            
            var dt = converter.ToDataTable();
            
            Assert.IsNotNull(dt);
            Assert.AreEqual(1, dt.Rows.Count); // Only header row
            Assert.AreEqual(3, dt.Columns.Count);
        }

        [TestMethod]
        public void TestNullDelimiter()
        {
            var file = @"test.csv";
            
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                var converter = new CsvTo.CsvConverter(file, delimiter: null);
            });
        }

        [TestMethod]
        public void TestCustomDelimiter()
        {
            // Create a semicolon-delimited CSV
            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, "a;b;c\n1;2;3\n4;5;6", Encoding.UTF8);
            
            try
            {
                var converter = new CsvTo.CsvConverter(tempFile, hasHeader: true, delimiter: ";");
                var dt = converter.ToDataTable();
                
                Assert.IsNotNull(dt);
                Assert.AreEqual(3, dt.Rows.Count);
                Assert.AreEqual(3, dt.Columns.Count);
                Assert.AreEqual("a", dt.Columns[0].ColumnName);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [TestMethod]
        public void TestTabDelimiter()
        {
            // Create a tab-delimited file
            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, "a\tb\tc\n1\t2\t3\n4\t5\t6", Encoding.UTF8);
            
            try
            {
                var converter = new CsvTo.CsvConverter(tempFile, hasHeader: true, delimiter: "\t");
                var dt = converter.ToDataTable();
                
                Assert.IsNotNull(dt);
                Assert.AreEqual(3, dt.Rows.Count);
                Assert.AreEqual("1", dt.Rows[1][0].ToString());
                Assert.AreEqual("2", dt.Rows[1][1].ToString());
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [TestMethod]
        public void TestSpecialCharactersInData()
        {
            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, "col1,col2,col3\n\"data,with,commas\",normal,\"data\nwith\nnewlines\"", Encoding.UTF8);
            
            try
            {
                var converter = new CsvTo.CsvConverter(tempFile, hasHeader: true);
                var collection = converter.ToCollection().ToList();
                
                Assert.IsNotNull(collection);
                Assert.AreEqual(1, collection.Count);
                Assert.AreEqual("data,with,commas", collection[0][0]);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [TestMethod]
        public void TestStreamInput()
        {
            var csvContent = "a,b,c\n1,2,3\n4,5,6";
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent)))
            {
                var converter = new CsvTo.CsvConverter(stream, hasHeader: true);
                var dt = converter.ToDataTable();
                
                Assert.IsNotNull(dt);
                Assert.AreEqual(3, dt.Rows.Count);
                Assert.AreEqual("1", dt.Rows[1][0].ToString());
            }
        }

        [TestMethod]
        public void TestStreamInputWithEncoding()
        {
            var csvContent = "姓名,年龄\n张三,25";
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent)))
            {
                var converter = new CsvTo.CsvConverter(stream, hasHeader: true, encoding: Encoding.UTF8);
                var collection = converter.ToCollection().ToList();
                
                Assert.IsNotNull(collection);
                Assert.AreEqual(1, collection.Count);
                Assert.AreEqual("张三", collection[0][0]);
                Assert.AreEqual("25", collection[0][1]);
            }
        }

        [TestMethod]
        public void TestFileNotFound()
        {
            Assert.ThrowsException<FileNotFoundException>(() =>
            {
                var converter = new CsvTo.CsvConverter("nonexistent_file.csv");
                var dt = converter.ToDataTable();
            });
        }

        [TestMethod]
        public void TestLargeFile()
        {
            // Create a large CSV file with 10000 rows
            var tempFile = Path.GetTempFileName();
            using (var writer = new StreamWriter(tempFile, false, Encoding.UTF8))
            {
                writer.WriteLine("id,name,value");
                for (int i = 0; i < 10000; i++)
                {
                    writer.WriteLine($"{i},name{i},{i * 100}");
                }
            }
            
            try
            {
                var converter = new CsvTo.CsvConverter(tempFile, hasHeader: true);
                var collection = converter.ToCollection().ToList();
                
                Assert.IsNotNull(collection);
                Assert.AreEqual(10000, collection.Count);
                Assert.AreEqual("0", collection[0][0]);
                Assert.AreEqual("9999", collection[9999][0]);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [TestMethod]
        public void TestReverseLargeFile()
        {
            // Create a large CSV file with 10000 rows
            var tempFile = Path.GetTempFileName();
            using (var writer = new StreamWriter(tempFile, false, Encoding.UTF8))
            {
                writer.WriteLine("id,name,value");
                for (int i = 0; i < 10000; i++)
                {
                    writer.WriteLine($"{i},name{i},{i * 100}");
                }
            }
            
            try
            {
                var reverseConverter = new CsvTo.CsvReverseConverter(tempFile, hasHeader: true);
                var collection = reverseConverter.ToCollection().ToList();
                
                Assert.IsNotNull(collection);
                Assert.AreEqual(10000, collection.Count);
                // Reverse should read last data row first
                Assert.AreEqual("9999", collection[0][0]);
                Assert.AreEqual("0", collection[9999][0]);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }
    }
}
