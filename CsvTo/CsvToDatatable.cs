using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace CsvTo
{
    public class CsvToDatatable
    {
        //private static readonly Regex csvParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
        private readonly CsvHandler<DataTable> csvHandler = new CsvHandler<DataTable>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="hasHeader"></param>
        /// <returns></returns>
        public async Task<DataTable> ConvertFromFile(string filePath, bool hasHeader)
        {
            DataTable dt = new DataTable();
            await csvHandler.Handler(dt, filePath, hasHeader, WithHeader, WithoutHeader, ElementHandler);
            return dt;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="hasHeader"></param>
        /// <returns></returns>
        public async Task<DataTable> ConvertFromStream(Stream fileStream, bool hasHeader)
        {
            DataTable dt = new DataTable();
            await csvHandler.Handler(dt, fileStream, hasHeader, WithHeader, WithoutHeader, ElementHandler);
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
