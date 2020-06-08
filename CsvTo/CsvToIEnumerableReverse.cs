using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CsvTo
{
    public class CsvToIEnumerableReverse<T> where T : new()
    {
        string _filePath;
        bool _hasHeader;
        Stream _fileStream;
        Dictionary<int, string> _header;
        public CsvToIEnumerableReverse(string filePath, bool hasHeader)
        {
            _filePath = filePath;
            _hasHeader = hasHeader;
        }
        public CsvToIEnumerableReverse(Stream fileStream, bool hasHeader)
        {
            _fileStream = fileStream;
            _hasHeader = hasHeader;
        }
        public IEnumerable<T> ConvertFromFile()
        {
            CsvReverseHandler handler = new CsvReverseHandler(_filePath);
            if (!_hasHeader)
            {
                foreach (var item in handler)
                {
                    var elements = Parser.CsvParser.Split(item);

                    if (!elements.All(e => string.IsNullOrWhiteSpace(e)))
                    {
                        T result = new T();
                        elementHandler(result, elements);
                        yield return result;
                    }
                }
            }
            else
            {
                _header = Parser.CsvParser.Split(handler.Last()).Select((v, i) => new { key = i, value = v }).ToDictionary(k => k.key, v => v.value);
                for (int i = 0; i < handler.Count() - 1; i++)
                {
                    var elements = Parser.CsvParser.Split(handler.ElementAt(i));
                    if (!elements.All(e => string.IsNullOrWhiteSpace(e)))
                    {
                        T result = new T();
                        elementHandler(result, elements);
                        yield return result;
                    }
                }
            }
        }
        public IEnumerable<T> ConvertFromStream()
        {
            CsvReverseHandler handler = new CsvReverseHandler(_fileStream);
            if (!_hasHeader)
            {
                foreach (var item in handler)
                {
                    var elements = Parser.CsvParser.Split(item);

                    if (!elements.All(e => string.IsNullOrWhiteSpace(e)))
                    {
                        T result = new T();
                        elementHandler(result, elements);
                        yield return result;
                    }
                }
            }
            else
            {
                _header = Parser.CsvParser.Split(handler.Last()).Select((v, i) => new { key = i, value = v }).ToDictionary(k => k.key, v => v.value);
                for (int i = 0; i < handler.Count() - 1; i++)
                {
                    var elements = Parser.CsvParser.Split(handler.ElementAt(i));
                    if (!elements.All(e => string.IsNullOrWhiteSpace(e)))
                    {
                        T result = new T();
                        elementHandler(result, elements);
                        yield return result;
                    }
                }
            }
        }

        private void elementHandler(T result, string[] elements)
        {
            var ty = typeof(T);
            var props = ty.GetProperties();
            if (_hasHeader)
            {
                for (int i = 0; i < _header.Count(); i++)
                {
                    var h = _header.ElementAt(i);
                    //prots[i].SetValue(result, Convert.ChangeType(elements[i], prots[i].PropertyType));
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
    }
}
