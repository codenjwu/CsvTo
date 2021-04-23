using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CsvTo
{
    public class CsvToIEnumerableReverse 
    {
        public IEnumerable<string[]> ConvertFromFile(string filePath, bool hasHeader = false, string delimiter = ",", string escape = "\"")
        {
            CsvReverseHandler handler = new CsvReverseHandler(filePath, delimiter, escape);
            if (!hasHeader)
            {
                foreach (var item in handler)
                {
                    var elements = handler.Parser.Split(item);

                    if (!elements.All(e => string.IsNullOrWhiteSpace(e)))
                    {
                        yield return elements;
                    }
                }
            }
            else
            {
                var tmpQueue = new Queue<string[]>();
                var er = handler.GetEnumerator();
                while (er.MoveNext())
                {
                    var elements = handler.Parser.Split(er.Current);

                    if (!elements.All(e => string.IsNullOrWhiteSpace(e)))
                    {
                        tmpQueue.Enqueue(elements);
                        if (tmpQueue.Count > 1)
                           yield return tmpQueue.Dequeue();
                    }
                }
            }
        }
        public IEnumerable<string[]> ConvertFromStream(Stream fileStream, bool hasHeader = false, string delimiter = ",", string escape = "\"")
        {
            CsvReverseHandler handler = new CsvReverseHandler(fileStream, delimiter, escape);
            if (!hasHeader)
            {
                foreach (var item in handler)
                {
                    var elements = handler.Parser.Split(item);

                    if (!elements.All(e => string.IsNullOrWhiteSpace(e)))
                    {
                        yield return elements;
                    }
                }
            }
            else
            {
                var tmpQueue = new Queue<string[]>();
                var er = handler.GetEnumerator();
                while (er.MoveNext())
                {
                    var elements = handler.Parser.Split(er.Current);

                    if (!elements.All(e => string.IsNullOrWhiteSpace(e)))
                    {
                        tmpQueue.Enqueue(elements);
                        if (tmpQueue.Count > 1)
                            yield return tmpQueue.Dequeue();
                    }
                }
            }
        }
    }
}
