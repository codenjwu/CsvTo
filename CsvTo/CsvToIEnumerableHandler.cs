using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CsvTo
{
    internal class CsvToIEnumerableHandler<T> where T : new()
    {
        internal Dictionary<int, string> Header;
        internal IEnumerable<T> Handler(string filePath, bool hasHeader, Action<StreamReader> withHeader, Action<StreamReader> WithoutHeader, Action<T, bool, string[]> elementHandler)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                if (hasHeader)
                    withHeader(reader);
                else
                    WithoutHeader(reader);
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLineAsync().GetAwaiter().GetResult();
                    string[] elements = Parser.CsvParser.Split(line);
                    if (!elements.All(e => string.IsNullOrWhiteSpace(e)))
                    {
                        T result = new T();
                        elementHandler(result, hasHeader, elements);
                        yield return result;
                    }
                }
            }
        }
        internal IEnumerable<T> Handler(Stream fileStream, bool hasHeader, Action<StreamReader> withHeader, Action<StreamReader> WithoutHeader, Action<T, bool, string[]> elementHandler)
        {
            using (StreamReader reader = new StreamReader(fileStream))
            {
                if (hasHeader)
                    withHeader(reader);
                else
                    WithoutHeader(reader);
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLineAsync().GetAwaiter().GetResult();
                    string[] elements = Parser.CsvParser.Split(line);
                    if (!elements.All(e => string.IsNullOrWhiteSpace(e)))
                    {
                        T result = new T();
                        elementHandler(result, hasHeader, elements);
                        yield return result;
                    }
                }
            }
        }
    }
}
