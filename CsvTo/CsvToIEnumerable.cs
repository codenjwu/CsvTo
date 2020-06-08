using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CsvTo
{
    public class CsvToIEnumerable<T> where T : new()
    {
        string _filePath;
        bool _hasHeader;
        Stream _fileStream;
        public CsvToIEnumerable(string filePath, bool hasHeader)
        {
            _filePath = filePath;
            _hasHeader = hasHeader;
        }
        public CsvToIEnumerable(Stream fileStream, bool hasHeader)
        {
            _fileStream = fileStream;
            _hasHeader = hasHeader;
        }
        private readonly CsvToIEnumerableHandler<T> csvHandler = new CsvToIEnumerableHandler<T>();

        public IEnumerable<T> ConvertFromFile()
        {
            foreach (var item in csvHandler.Handler(_filePath, _hasHeader, withHeader, WithoutHeader, elementHandler))
                yield return item;
        }

        public IEnumerable<T> ConvertFromStream()
        {
            foreach (var item in csvHandler.Handler(_fileStream, _hasHeader, withHeader, WithoutHeader, elementHandler))
                yield return item;
        }
        private void WithoutHeader(StreamReader reader)
        {
            csvHandler.Header = Parser.CsvParser.Split(reader.ReadLineAsync().GetAwaiter().GetResult()).Select((v, i) => new { key = i, value = v }).ToDictionary(k => k.key, v => v.value);
            reader.BaseStream.Position = 0;
            reader.DiscardBufferedData();
        }
        private void elementHandler(T result, bool HasHeader, string[] elements)
        {
            var ty = typeof(T);
            var props = ty.GetProperties();
            if (HasHeader)
            {
                for (int i = 0; i < csvHandler.Header.Count; i++)
                {
                    var h = csvHandler.Header.ElementAt(i);
                    var prop = ty.GetProperty(h.Value, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    if (prop != null)
                        prop.SetValue(result, TypeConverter.ConvertType(elements[h.Key], prop.PropertyType));
                }
            }
            else
            {
                foreach (var prop in props)
                {
                    var attr = prop.GetCustomAttributes(typeof(CsvColumnIndexAttribute), false).FirstOrDefault() as CsvColumnIndexAttribute;
                    if (attr != null)
                    {
                        prop.SetValue(result, Convert.ChangeType(elements[attr.ColumnIndex - 1], prop.PropertyType));
                    }
                }
            }
        }

        private void withHeader(StreamReader reader)
        {
            csvHandler.Header = Parser.CsvParser.Split(reader.ReadLineAsync().GetAwaiter().GetResult()).Select((v, i) => new { key = i, value = v }).ToDictionary(k => k.key, v => v.value);
        }
    }
}
