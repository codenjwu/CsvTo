using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CsvTo
{
    public class CsvToDatatable
    {
        string _filePath;
        bool _hasHeader;
        Stream _fileStream;
        public CsvToDatatable(string filePath, bool hasHeader)
        {
            _filePath = filePath;
            _hasHeader = hasHeader;
        }
        public CsvToDatatable(Stream fileStream, bool hasHeader)
        {
            _fileStream = fileStream;
            _hasHeader = hasHeader;
        }

        private readonly CsvHandler<DataTable> csvHandler = new CsvHandler<DataTable>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="hasHeader"></param>
        /// <returns></returns>
        public async Task<DataTable> ConvertFromFile()
        {
            DataTable dt = new DataTable();
            await csvHandler.Handler(dt, _filePath, _hasHeader, WithHeader, WithoutHeader, ElementHandler);
            return dt;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="hasHeader"></param>
        /// <returns></returns>
        public async Task<DataTable> ConvertFromStream()
        {
            DataTable dt = new DataTable();
            await csvHandler.Handler(dt, _fileStream, _hasHeader, WithHeader, WithoutHeader, ElementHandler);
            return dt;
        }

        private void ElementHandler(DataTable dt, string[] elements)
        {
            dt.Rows.Add(elements);
        }

        private async Task WithoutHeader(DataTable dt, StreamReader reader)
        {
            var firstLine = Parser.CsvParser.Split(await reader.ReadLineAsync());
            dt.Columns.AddRange(firstLine.Select((f, i) => new DataColumn($"column{i}")).ToArray());
            dt.Rows.Add(firstLine);
        }

        private async Task WithHeader(DataTable dt, StreamReader reader)
        {
            DataColumn[] header = Parser.CsvParser.Split(await reader.ReadLineAsync()).Select(l => new DataColumn(l)).ToArray();
            dt.Columns.AddRange(header);
        }
    }
}
