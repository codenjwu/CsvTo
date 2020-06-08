using System.Data;
using System.IO;
using System.Linq;

namespace CsvTo
{
    public class CsvToDatatableReverse
    {
        string _filePath;
        bool _hasHeader;
        Stream _fileStream;
        public CsvToDatatableReverse(string filePath, bool hasHeader)
        {
            _filePath = filePath;
            _hasHeader = hasHeader;
        }
        public CsvToDatatableReverse(Stream fileStream, bool hasHeader)
        {
            _fileStream = fileStream;
            _hasHeader = hasHeader;
        }
        public DataTable ConvertFromFile()
        {
            DataTable dt = new DataTable();
            CsvReverseHandler handler = new CsvReverseHandler(_filePath);
            if (!_hasHeader)
            {
                var first = handler.FirstOrDefault();
                dt.Columns.AddRange(Parser.CsvParser.Split(first).Select((f, i) => new DataColumn($"column{i}")).ToArray());
                foreach (var item in handler)
                {
                    var elements = Parser.CsvParser.Split(item);

                    if (!elements.All(e => string.IsNullOrWhiteSpace(e)))
                    {
                        dt.Rows.Add(elements);
                    }
                }
            }
            else
            {
                var header = handler.Last();
                dt.Columns.AddRange(Parser.CsvParser.Split(header).Select(h => new DataColumn(h)).ToArray());
                for (int i = 0; i < handler.Count() - 1; i++)
                {
                    var elements = Parser.CsvParser.Split(handler.ElementAt(i));
                    if (!elements.All(e => string.IsNullOrWhiteSpace(e)))
                    {
                        dt.Rows.Add(elements);
                    }
                }
            }
            return dt;
        }
        public DataTable ConvertFromStream()
        {
            DataTable dt = new DataTable();
            CsvReverseHandler handler = new CsvReverseHandler(_filePath);
            if (!_hasHeader)
            {
                var first = handler.FirstOrDefault();
                dt.Columns.AddRange(Parser.CsvParser.Split(first).Select((f, i) => new DataColumn($"column{i}")).ToArray());
                foreach (var item in handler)
                {
                    var elements = Parser.CsvParser.Split(item);

                    if (!elements.All(e => string.IsNullOrWhiteSpace(e)))
                    {
                        dt.Rows.Add(elements);
                    }
                }
            }
            else
            {
                var header = handler.Last();
                dt.Columns.AddRange(Parser.CsvParser.Split(header).Select(h => new DataColumn(h)).ToArray());
                for (int i = 0; i < handler.Count() - 1; i++)
                {
                    var elements = Parser.CsvParser.Split(handler.ElementAt(i));
                    if (!elements.All(e => string.IsNullOrWhiteSpace(e)))
                    {
                        dt.Rows.Add(elements);
                    }
                }
            }
            return dt;
        }
    }
}
