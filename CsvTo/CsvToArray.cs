using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CsvTo
{
    public class CsvToArray
    {
        string _filePath;
        bool _hasHeader;
        Stream _fileStream;
        public CsvToArray(string filePath, bool hasHeader)
        {
            _filePath = filePath;
            _hasHeader = hasHeader;
        }
        public CsvToArray(Stream fileStream, bool hasHeader)
        {
            _fileStream = fileStream;
            _hasHeader = hasHeader;
        }
        private readonly CsvHandler<List<string[]>> csvHandler = new CsvHandler<List<string[]>>();
        public async Task<List<string[]>> ConvertFromFile()
        {
            List<string[]> vs = new List<string[]>();
            await csvHandler.Handler(vs, _filePath, _hasHeader, withHeader, withoutHeader, elementHandler);
            return vs;
        }

        public async Task<List<string[]>> ConvertFromStream()
        {
            List<string[]> vs = new List<string[]>();
            await csvHandler.Handler(vs, _fileStream, _hasHeader, withHeader, withoutHeader, elementHandler);
            return vs;
        }

        private void elementHandler(List<string[]> result, string[] elements) =>
            result.Add(elements);

        private async Task withoutHeader(List<string[]> result, StreamReader reader) { }

        private async Task withHeader(List<string[]> result, StreamReader reader) =>
            await reader.ReadLineAsync();
    }
}
