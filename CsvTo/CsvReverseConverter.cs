using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace CsvTo
{
    public class CsvReverseConverter
    {
        string _filePath = null;
        Stream _fileStream;
        bool _hasHeader;
        string _delimiter;
        string _escape;

        public CsvReverseConverter(string filePath, bool hasHeader = false, string delimiter = ",", string escape = "\"")
        {
            _filePath = filePath;
            _hasHeader = hasHeader;
            _delimiter = delimiter;
            _escape = escape;
        }
        public CsvReverseConverter(Stream fileStream, bool hasHeader = false, string delimiter = ",", string escape = "\"")
        {
            _fileStream = fileStream;
            _hasHeader = hasHeader;
            _delimiter = delimiter;
            _escape = escape;
        }
        public DataTable ToDataTable()
        {
            if (!string.IsNullOrWhiteSpace(_filePath))
                return ToDataTableFromFile();
            return ToDataTableFromStream();
        }
        public IEnumerable<string[]> ToCollection()
        {
            if (!string.IsNullOrWhiteSpace(_filePath))
                return ToCollectionFromFile();
            return ToCollectionFromStream();
        }

        DataTable ToDataTableFromFile()
        {
            CsvReverseHandler handler = new CsvReverseHandler(_filePath, _delimiter, _escape);
            return new CsvReverseConvertHandler().ToDataTableHandler(handler, _hasHeader);
        }
        DataTable ToDataTableFromStream()
        {
            CsvReverseHandler handler = new CsvReverseHandler(_fileStream, _delimiter, _escape);
            return new CsvReverseConvertHandler().ToDataTableHandler(handler, _hasHeader);
        }

        IEnumerable<string[]> ToCollectionFromFile()
        {
            CsvReverseHandler handler = new CsvReverseHandler(_filePath, _delimiter, _escape);
            return new CsvReverseConvertHandler().ToCollectionHandler(handler, _hasHeader);
        }
        IEnumerable<string[]> ToCollectionFromStream()
        {
            CsvReverseHandler handler = new CsvReverseHandler(_fileStream,_delimiter, _escape);
            return new CsvReverseConvertHandler().ToCollectionHandler(handler, _hasHeader);
        }
    }
}
