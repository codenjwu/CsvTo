using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CsvTo
{
    public class CsvToArrayReverse
    {
        string _filePath;
        bool _hasHeader;
        Stream _fileStream;
        public CsvToArrayReverse(string filePath, bool hasHeader)
        {
            _filePath = filePath;
            _hasHeader = hasHeader;
        }
        public CsvToArrayReverse(Stream fileStream, bool hasHeader)
        {
            _fileStream = fileStream;
            _hasHeader = hasHeader;
        }
        public List<string[]> ConvertFromFile()
        {
            List<string[]> res = new List<string[]>();
            CsvReverseHandler handler = new CsvReverseHandler(_filePath);
            foreach (var item in handler)
            {
                var elements = Parser.CsvParser.Split(item);
                if (!elements.All(e => string.IsNullOrWhiteSpace(e)))
                {
                    res.Add(elements);
                }
            }
            if (_hasHeader)
            {
                var header = res.Last();
                res.RemoveAt(res.Count - 1);
            }
            return res;
        }
        public List<string[]> ConvertFromStream()
        {
            List<string[]> res = new List<string[]>();
            CsvReverseHandler handler = new CsvReverseHandler(_fileStream);
            foreach (var item in handler)
            {
                var elements = Parser.CsvParser.Split(item);
                if (!elements.All(e => string.IsNullOrWhiteSpace(e)))
                {
                    res.Add(elements);
                }
            }
            if (_hasHeader)
                res.RemoveAt(res.Count - 1);
            return res;
        }
    }
}
