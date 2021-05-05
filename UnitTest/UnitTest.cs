using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Linq;

namespace UnitTest
{
    [TestClass]
    public class UnitTest
    {
        CsvTo.CsvConverter converter;
        CsvTo.CsvConverter<Test1> g_converter_1;
        CsvTo.CsvConverter<Test2> g_converter_2;
        CsvTo.CsvReverseConverter r_converter;
        CsvTo.CsvReverseConverter<Test1> g_r_converter_1;
        CsvTo.CsvReverseConverter<Test2> g_r_converter_2;
        [TestInitialize]
        public void Init()
        {
            var file = @"test.csv";
            var file1 = @"test_generic.csv";
            converter = new CsvTo.CsvConverter(file);
            r_converter = new CsvTo.CsvReverseConverter(file);
            g_converter_1 = new CsvTo.CsvConverter<Test1>(file1);
            g_converter_2 = new CsvTo.CsvConverter<Test2>(file1);
            g_r_converter_1 = new CsvTo.CsvReverseConverter<Test1>(file1);
            g_r_converter_2 = new CsvTo.CsvReverseConverter<Test2>(file1);
        }
        [TestMethod]
        public void TestMethod1()
        {
            var dt = converter.ToDataTable();

            var r_c = r_converter.ToCollection().ToList();

            var g_dt_1 = g_converter_1.ToDataTable();
            var g_dt_2 = g_converter_2.ToDataTable();
            var g_c_1 = g_converter_1.ToCollection().ToList();
            var g_c_2 = g_converter_2.ToCollection().ToList();

            var g_r_dt_1 = g_r_converter_1.ToDataTable();
            var g_r_dt_2 = g_r_converter_2.ToDataTable();
            var g_r_c_1 = g_r_converter_1.ToCollection().ToList();
            var g_r_c_2 = g_r_converter_2.ToCollection().ToList();
        }
    }
}
