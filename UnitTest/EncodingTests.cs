using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Linq;
using System.Text;

namespace UnitTest
{
    [TestClass]
    public class EncodingTests
    {
        [TestMethod]
        public void TestChineseEncodingUTF8()
        {
            var file = @"test_chinese.csv";
            var converter = new CsvTo.CsvConverter(file, hasHeader: true, encoding: Encoding.UTF8);
            
            var dt = converter.ToDataTable();
            
            Assert.IsNotNull(dt);
            Assert.AreEqual(4, dt.Rows.Count); // 3 data rows + 1 header
            Assert.AreEqual("姓名", dt.Columns[0].ColumnName);
            Assert.AreEqual("年龄", dt.Columns[1].ColumnName);
            Assert.AreEqual("城市", dt.Columns[2].ColumnName);
            
            Assert.AreEqual("张三", dt.Rows[1][0].ToString());
            Assert.AreEqual("25", dt.Rows[1][1].ToString());
            Assert.AreEqual("北京", dt.Rows[1][2].ToString());
        }

        [TestMethod]
        public void TestJapaneseEncodingUTF8()
        {
            var file = @"test_japanese.csv";
            var converter = new CsvTo.CsvConverter(file, hasHeader: true, encoding: Encoding.UTF8);
            
            var dt = converter.ToDataTable();
            
            Assert.IsNotNull(dt);
            Assert.AreEqual(4, dt.Rows.Count);
            Assert.AreEqual("名前", dt.Columns[0].ColumnName);
            Assert.AreEqual("年齢", dt.Columns[1].ColumnName);
            Assert.AreEqual("都市", dt.Columns[2].ColumnName);
            
            Assert.AreEqual("田中太郎", dt.Rows[1][0].ToString());
            Assert.AreEqual("25", dt.Rows[1][1].ToString());
            Assert.AreEqual("東京", dt.Rows[1][2].ToString());
        }

        [TestMethod]
        public void TestDefaultEncodingIsUTF8()
        {
            var file = @"test_chinese.csv";
            // Test that default encoding works with UTF8 content
            var converter = new CsvTo.CsvConverter(file, hasHeader: true);
            
            var collection = converter.ToCollection().ToList();
            
            Assert.IsNotNull(collection);
            Assert.IsTrue(collection.Count > 0);
            Assert.AreEqual("张三", collection[0][0]);
        }

        [TestMethod]
        public void TestReverseConverterWithEncoding()
        {
            var file = @"test_chinese.csv";
            var converter = new CsvTo.CsvReverseConverter(file, hasHeader: true, encoding: Encoding.UTF8);
            
            var collection = converter.ToCollection().ToList();
            
            Assert.IsNotNull(collection);
            Assert.IsTrue(collection.Count > 0);
            // Reverse converter reads from bottom, so last data row should be first
            Assert.AreEqual("王五", collection[0][0]);
            Assert.AreEqual("28", collection[0][1]);
            Assert.AreEqual("深圳", collection[0][2]);
        }

        [TestMethod]
        public void TestSpanishEncodingUTF8()
        {
            var file = @"test_spanish.csv";
            var converter = new CsvTo.CsvConverter(file, hasHeader: true, encoding: Encoding.UTF8);
            
            var dt = converter.ToDataTable();
            
            Assert.IsNotNull(dt);
            Assert.IsTrue(dt.Rows.Count > 0);
            Assert.AreEqual("Nombre", dt.Columns[0].ColumnName);
            Assert.AreEqual("Edad", dt.Columns[1].ColumnName);
            Assert.AreEqual("Ciudad", dt.Columns[2].ColumnName);
            
            // Test Spanish special characters: ñ, á, é, í, ó, ú, ü
            Assert.AreEqual("José García", dt.Rows[0][0].ToString());
            Assert.AreEqual("28", dt.Rows[0][1].ToString());
            Assert.AreEqual("Barcelona", dt.Rows[0][2].ToString());
            
            Assert.AreEqual("María López", dt.Rows[1][0].ToString());
            Assert.AreEqual("32", dt.Rows[1][1].ToString());
            Assert.AreEqual("Madrid", dt.Rows[1][2].ToString());
            
            Assert.AreEqual("Señor Ramírez", dt.Rows[2][0].ToString());
            Assert.AreEqual("45", dt.Rows[2][1].ToString());
            Assert.AreEqual("Málaga", dt.Rows[2][2].ToString());
        }

        [TestMethod]
        public void TestSpanishWithSpecialCharacters()
        {
            var file = @"test_spanish.csv";
            var converter = new CsvTo.CsvConverter(file, hasHeader: true, encoding: Encoding.UTF8);
            
            var collection = converter.ToCollection().ToList();
            
            Assert.IsNotNull(collection);
            Assert.IsTrue(collection.Count >= 3);
            
            // Verify Spanish special characters are preserved
            Assert.IsTrue(collection[0][0].Contains("José"));
            Assert.IsTrue(collection[1][0].Contains("María"));
            Assert.IsTrue(collection[2][0].Contains("ñ")); // ñ in "Señor"
            Assert.IsTrue(collection[2][0].Contains("í")); // í in "Ramírez"
            Assert.IsTrue(collection[2][2].Contains("á")); // á in "Málaga"
        }
    }
}
