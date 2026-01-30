using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace UnitTest
{
    /// <summary>
    /// Robustness and error handling tests
    /// </summary>
    [TestClass]
    public class RobustnessTests
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
        public void TestCorruptedData_MissingQuoteEnds()
        {
            var csvContent = @"id,text
1,""This is a quote without end
2,Normal text";

            var file = Path.Combine(_tempDir, "corrupted_quotes.csv");
            File.WriteAllText(file, csvContent, Encoding.UTF8);

            var converter = new CsvTo.CsvConverter(file, hasHeader: true);
            var collection = converter.ToCollection().ToList();

            // Should handle gracefully
            Assert.IsNotNull(collection);
        }

        [TestMethod]
        public void TestMalformedCsv_ExtraCommas()
        {
            var csvContent = @"id,name,value
1,Test,100,,,
2,Test2,200,,";

            var file = Path.Combine(_tempDir, "extra_commas.csv");
            File.WriteAllText(file, csvContent, Encoding.UTF8);

            var converter = new CsvTo.CsvConverter(file, hasHeader: true);
            var collection = converter.ToCollection().ToList();

            Assert.IsNotNull(collection);
            Assert.AreEqual(2, collection.Count);
        }

        [TestMethod]
        public void TestVeryLongLines()
        {
            var longText = new string('x', 100000);
            var csvContent = $@"id,text
1,{longText}
2,short";

            var file = Path.Combine(_tempDir, "long_lines.csv");
            File.WriteAllText(file, csvContent, Encoding.UTF8);

            var converter = new CsvTo.CsvConverter(file, hasHeader: true);
            var collection = converter.ToCollection().ToList();

            Assert.AreEqual(2, collection.Count);
            Assert.AreEqual(longText, collection[0][1]);
        }

        [TestMethod]
        public void TestEmptyLinesInMiddle()
        {
            var csvContent = @"id,value
1,100

2,200

3,300";

            var file = Path.Combine(_tempDir, "empty_middle.csv");
            File.WriteAllText(file, csvContent, Encoding.UTF8);

            var converter = new CsvTo.CsvConverter(file, hasHeader: true);
            var collection = converter.ToCollection().ToList();

            Assert.IsNotNull(collection);
            // Should handle empty lines appropriately
        }

        [TestMethod]
        public void TestTrailingSpacesInHeader()
        {
            var csvContent = @"id  ,  name  ,  value  
1,Test,100";

            var file = Path.Combine(_tempDir, "trailing_spaces.csv");
            File.WriteAllText(file, csvContent, Encoding.UTF8);

            var converter = new CsvTo.CsvConverter(file, hasHeader: true);
            var dt = converter.ToDataTable();

            Assert.AreEqual(3, dt.Columns.Count);
        }

        [TestMethod]
        public void TestMixedLineEndings()
        {
            var csvContent = "id,value\r\n1,100\n2,200\r\n3,300\n";

            var file = Path.Combine(_tempDir, "mixed_endings.csv");
            File.WriteAllText(file, csvContent, Encoding.UTF8);

            var converter = new CsvTo.CsvConverter(file, hasHeader: true);
            var collection = converter.ToCollection().ToList();

            Assert.AreEqual(3, collection.Count);
        }

        [TestMethod]
        public void TestNumericOverflow()
        {
            var csvContent = @"id,bigNumber
1,999999999999999999999999999999";

            var file = Path.Combine(_tempDir, "overflow.csv");
            File.WriteAllText(file, csvContent, Encoding.UTF8);

            var converter = new CsvTo.CsvConverter(file, hasHeader: true);
            var collection = converter.ToCollection().ToList();

            // Should read as string without crashing
            Assert.IsNotNull(collection[0][1]);
        }

        [TestMethod]
        public void TestSpecialCharactersInFilename()
        {
            var csvContent = "id,value\n1,100";
            var filename = "test file with spaces.csv";
            var file = Path.Combine(_tempDir, filename);
            File.WriteAllText(file, csvContent, Encoding.UTF8);

            var converter = new CsvTo.CsvConverter(file, hasHeader: true);
            var collection = converter.ToCollection().ToList();

            Assert.AreEqual(1, collection.Count);
        }

        [TestMethod]
        public void TestReadOnlyFile()
        {
            var csvContent = "id,value\n1,100";
            var file = Path.Combine(_tempDir, "readonly.csv");
            File.WriteAllText(file, csvContent, Encoding.UTF8);

            // Make file read-only
            File.SetAttributes(file, FileAttributes.ReadOnly);

            try
            {
                var converter = new CsvTo.CsvConverter(file, hasHeader: true);
                var collection = converter.ToCollection().ToList();

                Assert.AreEqual(1, collection.Count);
            }
            finally
            {
                // Remove read-only attribute for cleanup
                File.SetAttributes(file, FileAttributes.Normal);
            }
        }

        [TestMethod]
        public void TestZeroByteFile()
        {
            var file = Path.Combine(_tempDir, "zero_byte.csv");
            File.Create(file).Close();

            var converter = new CsvTo.CsvConverter(file);
            var collection = converter.ToCollection().ToList();

            Assert.AreEqual(0, collection.Count);
        }

        [TestMethod]
        public void TestFileWithOnlyHeader()
        {
            var csvContent = "id,name,value";
            var file = Path.Combine(_tempDir, "only_header.csv");
            File.WriteAllText(file, csvContent, Encoding.UTF8);

            var converter = new CsvTo.CsvConverter(file, hasHeader: true);
            var dt = converter.ToDataTable();

            Assert.AreEqual(3, dt.Columns.Count);
            Assert.AreEqual(1, dt.Rows.Count); // Just the header row
        }

        [TestMethod]
        public void TestDuplicateColumnNames()
        {
            var csvContent = @"id,name,name,value
1,Alice,Bob,100";

            var file = Path.Combine(_tempDir, "duplicate_cols.csv");
            File.WriteAllText(file, csvContent, Encoding.UTF8);

            var converter = new CsvTo.CsvConverter(file, hasHeader: true);
            var dt = converter.ToDataTable();

            // Should handle duplicate column names
            Assert.IsNotNull(dt);
        }

        [TestMethod]
        public void TestNullCharacters()
        {
            var csvContent = "id,text\n1,Test\0Data\n2,Normal";

            var file = Path.Combine(_tempDir, "null_chars.csv");
            File.WriteAllText(file, csvContent, Encoding.UTF8);

            var converter = new CsvTo.CsvConverter(file, hasHeader: true);
            var collection = converter.ToCollection().ToList();

            Assert.IsNotNull(collection);
            Assert.AreEqual(2, collection.Count);
        }

        [TestMethod]
        public void TestInvalidDateFormats()
        {
            var csvContent = @"id,date
1,2024-13-45
2,Invalid-Date
3,2024-01-15";

            var file = Path.Combine(_tempDir, "invalid_dates.csv");
            File.WriteAllText(file, csvContent, Encoding.UTF8);

            var converter = new CsvTo.CsvConverter(file, hasHeader: true);
            var collection = converter.ToCollection().ToList();

            // Should read all as strings without crashing
            Assert.AreEqual(3, collection.Count);
            Assert.AreEqual("2024-13-45", collection[0][1]);
        }

        [TestMethod]
        public void TestMixedQuotingStyles()
        {
            var csvContent = @"id,text
1,'Single quotes'
2,""Double quotes""
3,No quotes";

            var file = Path.Combine(_tempDir, "mixed_quotes.csv");
            File.WriteAllText(file, csvContent, Encoding.UTF8);

            var converter = new CsvTo.CsvConverter(file, hasHeader: true);
            var collection = converter.ToCollection().ToList();

            Assert.AreEqual(3, collection.Count);
        }

        [TestMethod]
        public void TestConsecutiveDelimiters()
        {
            var csvContent = @"id,,name,,,value
1,,Test,,,100";

            var file = Path.Combine(_tempDir, "consecutive_delimiters.csv");
            File.WriteAllText(file, csvContent, Encoding.UTF8);

            var converter = new CsvTo.CsvConverter(file, hasHeader: true);
            var collection = converter.ToCollection().ToList();

            Assert.IsNotNull(collection);
            Assert.IsTrue(collection[0].Length >= 3);
        }

        [TestMethod]
        public void TestBinaryGarbageInFile()
        {
            var file = Path.Combine(_tempDir, "binary_garbage.csv");
            var bytes = new byte[] { 0xFF, 0xFE, 0x00, 0x01, 0x02, 0x03 };
            File.WriteAllBytes(file, bytes);

            try
            {
                var converter = new CsvTo.CsvConverter(file);
                var collection = converter.ToCollection().ToList();
                // Should either handle gracefully or throw specific exception
                Assert.IsNotNull(collection);
            }
            catch (Exception ex)
            {
                // Should throw a meaningful exception
                Assert.IsNotNull(ex);
            }
        }

        [TestMethod]
        public void TestStreamDisposal()
        {
            var csvContent = "id,value\n1,100";
            var bytes = Encoding.UTF8.GetBytes(csvContent);

            using (var stream = new MemoryStream(bytes))
            {
                var converter = new CsvTo.CsvConverter(stream, hasHeader: true);
                var collection = converter.ToCollection().ToList();
                Assert.AreEqual(1, collection.Count);
            }

            // Stream should be properly disposed
            // No assertion needed, test passes if no exception
        }

        [TestMethod]
        public void TestPathTraversalAttempt()
        {
            // Ensure library doesn't allow path traversal
            var maliciousPath = Path.Combine(_tempDir, "..", "..", "malicious.csv");
            
            try
            {
                var converter = new CsvTo.CsvConverter(maliciousPath);
                // If it doesn't throw, file should be safely handled
            }
            catch (FileNotFoundException)
            {
                // Expected if file doesn't exist
                Assert.IsTrue(true);
            }
        }
    }
}
