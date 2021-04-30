using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CsvTo
{
    public class CsvReverseConverter<T> where T : class, new()
    {
        string _filePath = null;
        Stream _fileStream;
        bool _hasHeader;
        string _delimiter;
        string _escape;
        static Type _type;
        static PropertyInfo[] _ps;
        // cache property that is not complex type and does not have a csvignore attribute
        static Dictionary<string, (int index, Type ty, PropertyDescriptor pd)> _props;
        static CsvReverseConverter()
        {
            _type = typeof(T);
            _ps = _type.GetProperties();
            _props = RefHelper.GetProperties(_type);
        }

        public CsvReverseConverter(string filePath, bool hasHeader = true, string delimiter = ",", string escape = "\"")
        {
            if (!hasHeader)
                throw new FormatException("Generic converter requests a header for csv file!");
            _filePath = filePath;
            _hasHeader = hasHeader;
            _delimiter = delimiter;
            _escape = escape;
        }
        public CsvReverseConverter(Stream fileStream, bool hasHeader = true, string delimiter = ",", string escape = "\"")
        {
            if (!hasHeader)
                throw new FormatException("Generic converter requests a header for csv file!");
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
        public IEnumerable<T> ToCollection()
        {
            if (!string.IsNullOrWhiteSpace(_filePath))
                return ToCollectionFromFile();
            return ToCollectionFromStream();
        }

        DataTable ToDataTableFromFile()
        {
            CsvReverseHandler handler = new CsvReverseHandler(_filePath, _delimiter, _escape);
            return new CsvReverseConvertHandler().ToDataTableHandler(handler, _props);
        }
        DataTable ToDataTableFromStream()
        {
            CsvReverseHandler handler = new CsvReverseHandler(_fileStream, _delimiter, _escape);
            return new CsvReverseConvertHandler().ToDataTableHandler(handler, _props);
        }

        IEnumerable<T> ToCollectionFromFile()
        {
            CsvReverseHandler handler = new CsvReverseHandler(_filePath, _delimiter, _escape);
            return new CsvReverseConvertHandler().ToCollectionHandler<T>(handler, _props);
        }
        IEnumerable<T> ToCollectionFromStream()
        {
            CsvReverseHandler handler = new CsvReverseHandler(_fileStream, _delimiter, _escape);
            return new CsvReverseConvertHandler().ToCollectionHandler<T>(handler, _props);
        }
        
    }
}
