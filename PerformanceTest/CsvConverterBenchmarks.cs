using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace PerformanceTest
{
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.Net90)]
    [SimpleJob(RuntimeMoniker.Net80)]
    public class CsvConverterBenchmarks
    {
        private string _smallFile = "small_benchmark.csv";
        private string _mediumFile = "medium_benchmark.csv";
        private string _largeFile = "large_benchmark.csv";

        [GlobalSetup]
        public void Setup()
        {
            // 创建小文件: 100 行
            CreateBenchmarkFile(_smallFile, 100);
            
            // 创建中等文件: 10,000 行
            CreateBenchmarkFile(_mediumFile, 10000);
            
            // 创建大文件: 100,000 行
            CreateBenchmarkFile(_largeFile, 100000);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            if (File.Exists(_smallFile)) File.Delete(_smallFile);
            if (File.Exists(_mediumFile)) File.Delete(_mediumFile);
            if (File.Exists(_largeFile)) File.Delete(_largeFile);
        }

        private void CreateBenchmarkFile(string filename, int rows)
        {
            using (var writer = new StreamWriter(filename, false, Encoding.UTF8))
            {
                // 写入表头
                writer.WriteLine("Id,Name,Email,Age,City,Country,Phone,Address");
                
                // 写入数据行
                for (int i = 1; i <= rows; i++)
                {
                    writer.WriteLine($"{i},User{i},user{i}@example.com,{20 + (i % 50)},City{i % 100},Country{i % 20},+1234567890{i % 1000},Address Line {i}");
                }
            }
        }

        // ========== 小文件测试 (100 行) ==========
        
        [Benchmark]
        public DataTable SmallFile_ToDataTable()
        {
            var converter = new CsvTo.CsvConverter(_smallFile);
            return converter.ToDataTable();
        }

        [Benchmark]
        public List<string[]> SmallFile_ToCollection()
        {
            var converter = new CsvTo.CsvConverter(_smallFile);
            return converter.ToCollection().ToList();
        }

        [Benchmark]
        public List<TestRecord> SmallFile_ToGenericCollection()
        {
            var converter = new CsvTo.CsvConverter<TestRecord>(_smallFile);
            return converter.ToCollection().ToList();
        }

        // ========== 中等文件测试 (10,000 行) ==========
        
        [Benchmark]
        public DataTable MediumFile_ToDataTable()
        {
            var converter = new CsvTo.CsvConverter(_mediumFile);
            return converter.ToDataTable();
        }

        [Benchmark]
        public List<string[]> MediumFile_ToCollection()
        {
            var converter = new CsvTo.CsvConverter(_mediumFile);
            return converter.ToCollection().ToList();
        }

        [Benchmark]
        public List<TestRecord> MediumFile_ToGenericCollection()
        {
            var converter = new CsvTo.CsvConverter<TestRecord>(_mediumFile);
            return converter.ToCollection().ToList();
        }

        // ========== 大文件测试 (100,000 行) ==========
        
        [Benchmark]
        public DataTable LargeFile_ToDataTable()
        {
            var converter = new CsvTo.CsvConverter(_largeFile);
            return converter.ToDataTable();
        }

        [Benchmark]
        public List<string[]> LargeFile_ToCollection()
        {
            var converter = new CsvTo.CsvConverter(_largeFile);
            return converter.ToCollection().ToList();
        }

        [Benchmark]
        public List<TestRecord> LargeFile_ToGenericCollection()
        {
            var converter = new CsvTo.CsvConverter<TestRecord>(_largeFile);
            return converter.ToCollection().ToList();
        }

        // ========== 编码性能测试 ==========
        
        [Benchmark]
        public DataTable UTF8_Encoding()
        {
            var converter = new CsvTo.CsvConverter(_mediumFile, encoding: Encoding.UTF8);
            return converter.ToDataTable();
        }

        [Benchmark]
        public DataTable UTF16_Encoding()
        {
            var converter = new CsvTo.CsvConverter(_mediumFile, encoding: Encoding.Unicode);
            return converter.ToDataTable();
        }

        // ========== 反向读取性能测试 ==========
        
        [Benchmark]
        public List<string[]> Reverse_SmallFile()
        {
            var converter = new CsvTo.CsvReverseConverter(_smallFile);
            return converter.ToCollection().ToList();
        }

        [Benchmark]
        public List<string[]> Reverse_MediumFile()
        {
            var converter = new CsvTo.CsvReverseConverter(_mediumFile);
            return converter.ToCollection().ToList();
        }
    }

    public class TestRecord
    {
        [CsvTo.CsvColumn("Id")]
        public int Id { get; set; }

        [CsvTo.CsvColumn("Name")]
        public string Name { get; set; } = string.Empty;

        [CsvTo.CsvColumn("Email")]
        public string Email { get; set; } = string.Empty;

        [CsvTo.CsvColumn("Age")]
        public int Age { get; set; }

        [CsvTo.CsvColumn("City")]
        public string City { get; set; } = string.Empty;

        [CsvTo.CsvColumn("Country")]
        public string Country { get; set; } = string.Empty;

        [CsvTo.CsvColumn("Phone")]
        public string Phone { get; set; } = string.Empty;

        [CsvTo.CsvColumn("Address")]
        public string Address { get; set; } = string.Empty;
    }
}
