using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CsvTo
{
    internal class CsvHandler<T>
    {
        internal async Task<T> Handler(T result, string filePath, bool hasHeader, Func<T, StreamReader, Task> withHeader, Func<T, StreamReader, Task> withoutHeader, Action<T, string[]> elementHandler)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                if (hasHeader)
                    await withHeader(result, reader);
                else
                    await withoutHeader(result, reader);
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    string[] elements = Parser.CsvParser.Split(line);
                    if (!elements.All(e => string.IsNullOrWhiteSpace(e)))
                    {
                        elementHandler(result, elements);
                    }
                }
            }
            return result;
        }
        internal async Task<T> Handler(T result, Stream fileStream, bool hasHeader, Func<T, StreamReader, Task> withHeader, Func<T, StreamReader, Task> withoutHeader, Action<T, string[]> elementHandler)
        {
            using (StreamReader reader = new StreamReader(fileStream))
            {
                if (hasHeader)
                    await withHeader(result, reader);
                else
                    await withoutHeader(result, reader);
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    string[] elements = Parser.CsvParser.Split(line);
                    if (!elements.All(e => string.IsNullOrWhiteSpace(e)))
                    {
                        elementHandler(result, elements);
                    }
                }
            }
            return result;
        }
    }
}
