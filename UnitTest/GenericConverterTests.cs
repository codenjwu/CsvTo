using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using System.Linq;
using System.Text;

namespace UnitTest
{
    public class PersonTest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string City { get; set; }
    }

    public class PersonTestWithNullable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? Age { get; set; }
        public string City { get; set; }
    }

    [TestClass]
    public class GenericConverterTests
    {
        [TestMethod]
        public void TestGenericConverterBasic()
        {
            var converter = new CsvTo.CsvConverter<Test1>(@"test_generic.csv");
            var collection = converter.ToCollection().ToList();
            
            Assert.IsNotNull(collection);
            Assert.IsTrue(collection.Count > 0);
            Assert.AreEqual(1, collection[0].int1);
        }

        [TestMethod]
        public void TestGenericConverterToDataTable()
        {
            var converter = new CsvTo.CsvConverter<Test1>(@"test_generic.csv");
            var dt = converter.ToDataTable();
            
            Assert.IsNotNull(dt);
            Assert.IsTrue(dt.Rows.Count > 0);
            Assert.IsTrue(dt.Columns.Count > 0);
        }

        [TestMethod]
        public void TestGenericConverterWithPartialColumns()
        {
            var converter = new CsvTo.CsvConverter<Test2>(@"test_generic.csv");
            var collection = converter.ToCollection().ToList();
            
            Assert.IsNotNull(collection);
            Assert.IsTrue(collection.Count > 0);
            Assert.AreEqual(1, collection[0].int1);
        }

        [TestMethod]
        public void TestGenericConverterRequiresHeader()
        {
            Assert.ThrowsException<FormatException>(() =>
            {
                var converter = new CsvTo.CsvConverter<Test1>(@"test_generic.csv", hasHeader: false);
            });
        }

        [TestMethod]
        public void TestGenericReverseConverter()
        {
            var converter = new CsvTo.CsvReverseConverter<Test1>(@"test_generic.csv");
            var collection = converter.ToCollection().ToList();
            
            Assert.IsNotNull(collection);
            Assert.IsTrue(collection.Count > 0);
        }

        [TestMethod]
        public void TestGenericReverseConverterToDataTable()
        {
            var converter = new CsvTo.CsvReverseConverter<Test1>(@"test_generic.csv");
            var dt = converter.ToDataTable();
            
            Assert.IsNotNull(dt);
            Assert.IsTrue(dt.Rows.Count > 0);
        }

        [TestMethod]
        public void TestCsvAttributeMapping()
        {
            var converter = new CsvTo.CsvConverter<Test1>(@"test_generic.csv");
            var collection = converter.ToCollection().ToList();
            
            Assert.IsNotNull(collection);
            Assert.IsTrue(collection.Count > 0);
            // Test that mapped properties work correctly
            Assert.IsTrue(collection[0].mapped_prop1 > 0);
        }

        [TestMethod]
        public void TestNullableProperties()
        {
            var converter = new CsvTo.CsvConverter<Test1>(@"test_generic.csv");
            var collection = converter.ToCollection().ToList();
            
            Assert.IsNotNull(collection);
            Assert.IsTrue(collection.Count > 0);
            // Test nullable int properties
            Assert.IsNotNull(collection[0].int3);
        }

        [TestMethod]
        public void TestDateTimeConversion()
        {
            var converter = new CsvTo.CsvConverter<Test1>(@"test_generic.csv");
            var collection = converter.ToCollection().ToList();
            
            Assert.IsNotNull(collection);
            Assert.IsTrue(collection.Count > 0);
            // Test DateTime conversion
            Assert.IsTrue(collection[0].datetime1 > DateTime.MinValue);
        }

        [TestMethod]
        public void TestDecimalConversion()
        {
            var converter = new CsvTo.CsvConverter<Test1>(@"test_generic.csv");
            var collection = converter.ToCollection().ToList();
            
            Assert.IsNotNull(collection);
            Assert.IsTrue(collection.Count > 0);
            // Test decimal conversion
            Assert.IsTrue(collection[0].decimal1.HasValue);
            Assert.IsTrue(collection[0].decimal1.Value > 0);
        }

        [TestMethod]
        public void TestDoubleConversion()
        {
            var converter = new CsvTo.CsvConverter<Test1>(@"test_generic.csv");
            var collection = converter.ToCollection().ToList();
            
            Assert.IsNotNull(collection);
            Assert.IsTrue(collection.Count > 0);
            // Test double conversion
            Assert.IsTrue(collection[0].double1 > 0);
        }
    }
}
