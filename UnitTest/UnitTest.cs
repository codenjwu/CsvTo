using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Linq;

namespace UnitTest
{
    [TestClass]
    public class UnitTest
    {
        CsvTo.CsvConverter converter;
        CsvTo.CsvReverseConverter r_converter;
        [TestInitialize]
        public void Init()
        {
            var file = @"test.csv";
            converter = new CsvTo.CsvConverter(file);
            r_converter = new CsvTo.CsvReverseConverter(file);
        }
        [TestMethod]
        public void TestMethod1()
        {
            var dt = converter.ToDataTable();

            var r_c = r_converter.ToCollection().ToList();
        }
    }
}
