using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CsvTo
{
    public class CsvToIEnumerable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="hasHeader"></param>
        /// <returns></returns>
        public IEnumerable<string[]> ConvertFromFile(string filePath, bool hasHeader = false, string delimiter = ",", string escape = "\"")
        {
            CsvHandler csvHandler = new CsvHandler(filePath, delimiter, escape);
            IEnumerable<string[]> cl = new List<string[]>();
            var er = csvHandler.GetEnumerator();
            if (hasHeader)
            {
                er.MoveNext();
            }
            while (er.MoveNext())
            {
                var elements = csvHandler.Parser.Split(er.Current);
                if (!elements.All(e => string.IsNullOrWhiteSpace(e)))
                    cl.Append(elements);
            }
            return cl;
        }
        public IEnumerable<string[]> ConvertFromStream(Stream fileStream, bool hasHeader = false, string delimiter = ",", string escape = "\"")
        {
            CsvHandler csvHandler = new CsvHandler(fileStream, delimiter, escape);
            IEnumerable<string[]> cl = new List<string[]>();
            var er = csvHandler.GetEnumerator();
            if (hasHeader)
            {
                er.MoveNext();
            }
            while (er.MoveNext())
            {
                var elements = csvHandler.Parser.Split(er.Current);
                if (!elements.All(e => string.IsNullOrWhiteSpace(e)))
                    cl.Append(elements);
            }
            return cl;
        }
    }
}
