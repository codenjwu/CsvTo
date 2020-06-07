using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CsvTo
{
    public class CsvToArray
    {
        private readonly CsvHandler<List<string[]>> csvHandler = new CsvHandler<List<string[]>>();
        public async Task<List<string[]>> ConvertFromFile(string filePath, bool hasHeader)
        {
            List<string[]> vs = new List<string[]>();
            await csvHandler.Handler(vs, filePath, hasHeader, withHeader, withoutHeader, elementHandler);
            return vs;
        }

        public async Task<List<string[]>> ConvertFromStream(Stream fileStream, bool hasHeader)
        {
            List<string[]> vs = new List<string[]>();
            await csvHandler.Handler(vs, fileStream, hasHeader, withHeader, withoutHeader, elementHandler);
            return vs;
        }

        private void elementHandler(List<string[]> result, string[] elements) =>
            result.Add(elements);

        private async Task withoutHeader(List<string[]> result, StreamReader reader) { }

        private async Task withHeader(List<string[]> result, StreamReader reader) =>
            await reader.ReadLineAsync();
    }
}
