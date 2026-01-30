using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Linq;

namespace UnitTest
{
    [TestClass]
    public class DataTableTests
    {
        [TestMethod]
        public void TestDataTableColumnCount()
        {
            var converter = new CsvTo.CsvConverter(@"test.csv");
            var dt = converter.ToDataTable();
            
            Assert.IsNotNull(dt);
            Assert.AreEqual(3, dt.Columns.Count);
        }

        [TestMethod]
        public void TestDataTableRowCount()
        {
            var converter = new CsvTo.CsvConverter(@"test.csv");
            var dt = converter.ToDataTable();
            
            Assert.IsNotNull(dt);
            Assert.IsTrue(dt.Rows.Count > 0);
        }

        [TestMethod]
        public void TestDataTableWithHeader()
        {
            var converter = new CsvTo.CsvConverter(@"test_generic.csv", hasHeader: true);
            var dt = converter.ToDataTable();
            
            Assert.IsNotNull(dt);
            Assert.AreEqual("int1", dt.Columns[0].ColumnName);
            Assert.AreEqual("int2", dt.Columns[1].ColumnName);
        }

        [TestMethod]
        public void TestDataTableWithoutHeader()
        {
            var converter = new CsvTo.CsvConverter(@"test.csv", hasHeader: false);
            var dt = converter.ToDataTable();
            
            Assert.IsNotNull(dt);
            Assert.IsTrue(dt.Columns.Count > 0);
            // Without header, column names should be auto-generated
            Assert.IsTrue(dt.Columns[0].ColumnName.StartsWith("Column"));
        }

        [TestMethod]
        public void TestDataTableCellValues()
        {
            var converter = new CsvTo.CsvConverter(@"test.csv", hasHeader: false);
            var dt = converter.ToDataTable();
            
            Assert.IsNotNull(dt);
            Assert.IsTrue(dt.Rows.Count > 0);
            Assert.IsNotNull(dt.Rows[0][0]);
        }

        [TestMethod]
        public void TestReverseDataTable()
        {
            var converter = new CsvTo.CsvConverter(@"test.csv");
            var reverseConverter = new CsvTo.CsvReverseConverter(@"test.csv");
            
            var dt = converter.ToDataTable();
            var rdt = reverseConverter.ToDataTable();
            
            Assert.IsNotNull(dt);
            Assert.IsNotNull(rdt);
            Assert.AreEqual(dt.Rows.Count, rdt.Rows.Count);
            Assert.AreEqual(dt.Columns.Count, rdt.Columns.Count);
        }

        [TestMethod]
        public void TestGenericDataTableColumnTypes()
        {
            var converter = new CsvTo.CsvConverter<Test1>(@"test_generic.csv");
            var dt = converter.ToDataTable();
            
            Assert.IsNotNull(dt);
            // Check that column types are correctly set
            var int1Column = dt.Columns["int1"];
            Assert.IsNotNull(int1Column);
            Assert.AreEqual(typeof(int), int1Column.DataType);
        }

        [TestMethod]
        public void TestDataTablePreservesData()
        {
            var converter = new CsvTo.CsvConverter(@"test.csv");
            var collection = converter.ToCollection().ToList();
            var dt = converter.ToDataTable();
            
            Assert.IsNotNull(collection);
            Assert.IsNotNull(dt);
            
            // Ensure row count matches
            Assert.AreEqual(collection.Count, dt.Rows.Count);
        }
    }
}
