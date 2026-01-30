using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace UnitTest
{
    [TestClass]
    public class CollectionTests
    {
        [TestMethod]
        public void TestCollectionNotNull()
        {
            var converter = new CsvTo.CsvConverter(@"test.csv");
            var collection = converter.ToCollection();
            
            Assert.IsNotNull(collection);
        }

        [TestMethod]
        public void TestCollectionHasItems()
        {
            var converter = new CsvTo.CsvConverter(@"test.csv");
            var collection = converter.ToCollection().ToList();
            
            Assert.IsNotNull(collection);
            Assert.IsTrue(collection.Count > 0);
        }

        [TestMethod]
        public void TestCollectionItemIsStringArray()
        {
            var converter = new CsvTo.CsvConverter(@"test.csv");
            var collection = converter.ToCollection().ToList();
            
            Assert.IsNotNull(collection);
            Assert.IsTrue(collection.Count > 0);
            Assert.IsInstanceOfType(collection[0], typeof(string[]));
        }

        [TestMethod]
        public void TestCollectionItemLength()
        {
            var converter = new CsvTo.CsvConverter(@"test.csv");
            var collection = converter.ToCollection().ToList();
            
            Assert.IsNotNull(collection);
            Assert.IsTrue(collection.Count > 0);
            Assert.AreEqual(3, collection[0].Length);
        }

        [TestMethod]
        public void TestReverseCollection()
        {
            var converter = new CsvTo.CsvReverseConverter(@"test.csv");
            var collection = converter.ToCollection().ToList();
            
            Assert.IsNotNull(collection);
            Assert.IsTrue(collection.Count > 0);
        }

        [TestMethod]
        public void TestReverseCollectionOrder()
        {
            var forwardConverter = new CsvTo.CsvConverter(@"test.csv");
            var reverseConverter = new CsvTo.CsvReverseConverter(@"test.csv");
            
            var forward = forwardConverter.ToCollection().ToList();
            var reverse = reverseConverter.ToCollection().ToList();
            
            Assert.IsNotNull(forward);
            Assert.IsNotNull(reverse);
            Assert.AreEqual(forward.Count, reverse.Count);
            
            // First row in forward should be last in reverse
            Assert.AreEqual(forward[0][0], reverse[reverse.Count - 1][0]);
            Assert.AreEqual(forward[forward.Count - 1][0], reverse[0][0]);
        }

        [TestMethod]
        public void TestGenericCollection()
        {
            var converter = new CsvTo.CsvConverter<Test1>(@"test_generic.csv");
            var collection = converter.ToCollection().ToList();
            
            Assert.IsNotNull(collection);
            Assert.IsTrue(collection.Count > 0);
            Assert.IsInstanceOfType(collection[0], typeof(Test1));
        }

        [TestMethod]
        public void TestGenericReverseCollection()
        {
            var converter = new CsvTo.CsvReverseConverter<Test1>(@"test_generic.csv");
            var collection = converter.ToCollection().ToList();
            
            Assert.IsNotNull(collection);
            Assert.IsTrue(collection.Count > 0);
            Assert.IsInstanceOfType(collection[0], typeof(Test1));
        }

        [TestMethod]
        public void TestCollectionIsEnumerable()
        {
            var converter = new CsvTo.CsvConverter(@"test.csv");
            var collection = converter.ToCollection();
            
            // Test that we can iterate without calling ToList()
            int count = 0;
            foreach (var item in collection)
            {
                count++;
                Assert.IsNotNull(item);
            }
            
            Assert.IsTrue(count > 0);
        }

        [TestMethod]
        public void TestCollectionCanBeEnumeratedMultipleTimes()
        {
            var converter = new CsvTo.CsvConverter(@"test.csv");
            var collection = converter.ToCollection();
            
            var firstCount = collection.Count();
            var secondCount = collection.Count();
            
            Assert.AreEqual(firstCount, secondCount);
        }

        [TestMethod]
        public void TestCollectionWithHeaderSkipsHeader()
        {
            var converter = new CsvTo.CsvConverter(@"test_generic.csv", hasHeader: true);
            var collection = converter.ToCollection().ToList();
            
            Assert.IsNotNull(collection);
            // With header, the first row should not be the header
            // It should be data
            Assert.IsFalse(collection[0][0] == "int1");
        }

        [TestMethod]
        public void TestCollectionWithoutHeaderIncludesAll()
        {
            var converter = new CsvTo.CsvConverter(@"test_generic.csv", hasHeader: false);
            var collection = converter.ToCollection().ToList();
            
            Assert.IsNotNull(collection);
            // Without header flag, first row should be the header data
            Assert.AreEqual("int1", collection[0][0]);
        }
    }
}
